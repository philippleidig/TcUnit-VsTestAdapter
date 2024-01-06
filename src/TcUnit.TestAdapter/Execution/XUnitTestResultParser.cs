using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using TcUnit.TestAdapter.RunSettings;
using System.Xml.Serialization;

namespace TcUnit.TestAdapter.Execution
{
    public class XUnitTestResultParser
    {
        private const string XUnitXmlSchemaResource = "TcUnit.TestAdapter.Schemas.XUnitXmlSchema.xsd";

        public IEnumerable<TestCaseResult> ParseFromFile (string filePath)
        {
            if(!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            FileStream fs = File.OpenRead(filePath);
            return Parse(fs);
        }

        public IEnumerable<TestCaseResult> Parse(Stream stream)
        {
            try
            {
                return ParseTestResults(stream);
            }
            catch (XmlException e)
            {
                throw new InvalidXUnitTestResultsException(e.Message, e);
            }
            catch (XmlSchemaValidationException e)
            {
                throw new InvalidXUnitTestResultsException(e.Message, e);
            }
            catch (InvalidOperationException e) when (e.InnerException is XmlSchemaValidationException)
            {
                throw new InvalidXUnitTestResultsException(e.InnerException.Message, e.InnerException);
            }
        }

        private IEnumerable<TestCaseResult> ParseTestResults(Stream stream)
        {
            var testCaseResults = new List<TestCaseResult>();

            XDocument testResults = XDocument.Load(stream);

            var schemaSet = new XmlSchemaSet();
            var schemaStream = Assembly.GetExecutingAssembly()
                                        .GetManifestResourceStream(XUnitXmlSchemaResource);

            schemaSet.Add(null, XmlReader.Create(schemaStream));
            testResults.Validate(schemaSet, (object o, ValidationEventArgs e) => throw e.Exception, true);

            foreach (XElement testSuite in testResults.Elements("testsuites").Elements("testsuite"))
            {
                var results = ParseTestSuiteResult(testSuite);
                testCaseResults.AddRange(results);
            }

            return testCaseResults;
        }

        private IEnumerable<TestCaseResult> ParseTestSuiteResult(XElement testSuiteElement)
        {
            var testSuiteName = testSuiteElement.Attribute("name").Value;

            foreach (XElement testCase in testSuiteElement.Elements("testcase"))
            {
                yield return ParseTestResult(testCase, testSuiteName);
            }
        }

        private TestCaseResult ParseTestResult(XElement testCaseElement, string testSuiteName)
        {
            var testName = testCaseElement.Attribute("name").Value;

            var testCaseName = string.Concat(testSuiteName, ".", testName);

            var result = new TestCaseResult();
            result.TestSuiteName = testSuiteName;
            result.Name = testName;
            result.FullyQualifiedName = testCaseName;

            var status = testCaseElement.Attribute("status").Value;

            if (status != "PASS")
            {
                var failure = testCaseElement.Element("failure");
                var message = failure.Attribute("message").Value;

                result.ErrorMessage = message;
                result.Outcome = TestOutcome.Failed;
            }
            else
            {
                result.Outcome = TestOutcome.Passed;
            }

            try
            {
                var duration = Convert.ToDouble(testCaseElement.Attribute("time").Value);
                result.Duration = TimeSpan.FromSeconds(duration);
            }
            catch { }

            return result;
        }

        [Serializable]
        public class InvalidXUnitTestResultsException : Exception
        {
            public InvalidXUnitTestResultsException() { }
            public InvalidXUnitTestResultsException(string message) : base(message) { }
            public InvalidXUnitTestResultsException(string message, Exception inner) : base(message, inner) { }
            protected InvalidXUnitTestResultsException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
}
