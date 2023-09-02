using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace TcUnit.TestAdapter.Execution
{
    internal class TestResultCollector
    {
        public IEnumerable<TestResult> CollectTestResults(string target, string filePath, IEnumerable<TestCase> tests)
        {

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Could not find test result xml.");
            }

            var testResults = new List<TestResult>();

            // load file from target via ads

            XDocument xdocument = XDocument.Load(filePath);

            foreach (XElement testSuite in xdocument.Elements("testsuites").Elements("testsuite"))
            {
                var testSuiteName = testSuite.Attribute("name").Value;

                foreach (XElement xelement in testSuite.Elements("testcase"))
                {
                    var testName = xelement.Attribute("name").Value;

                    var testCaseName = string.Concat(testSuiteName, ".", testName);

                    var testCase = tests.Where(t => t.DisplayName.Contains(testCaseName));
                    var result = new TestResult(testCase.First());

                    var status = xelement.Attribute("status").Value;

                    if (status != "PASS")
                    {
                        var failure = xelement.Element("failure");
                        var message = failure.Attribute("message").Value;

                        result.ErrorMessage = message;
                        result.Outcome = TestOutcome.Failed;
                    }
                    else
                    {
                        result.Outcome = TestOutcome.Passed;
                    }

                    var duration = Convert.ToDouble(xelement.Attribute("time").Value);
                    result.Duration = TimeSpan.FromSeconds(duration);

                    testResults.Add(result);
                }
            }

            return testResults;
        }
    }
}
