using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace TcUnit.TestAdapter.Execution
{
    public class XUnitTestResultParser
    {
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
            return ParseTestResults(stream);
        }

        private IEnumerable<TestCaseResult> ParseTestResults(Stream stream)
        {
            var testCaseResults = new List<TestCaseResult>();

            XDocument testResults = XDocument.Load(stream);

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
    }
}
