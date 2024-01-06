using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Text;
using TcUnit.TestAdapter.Execution;
using static TcUnit.TestAdapter.Execution.XUnitTestResultParser;

namespace TcUnit.TestAdapter.Execution
{
    [TestClass]
    public class XUnitTestResultParserTests
    {
        [TestMethod]
        public void TestParseTestResultsEmpty()
        {
            var xUnitTestResults = "";

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(xUnitTestResults));
            XUnitTestResultParser parser = new XUnitTestResultParser();

            Assert.ThrowsException<InvalidXUnitTestResultsException>(() => {
                IEnumerable<TestCaseResult> testResults = parser.Parse(ms);
            });
        }

        [TestMethod]
        public void TestParseTestResultsInvalidStatusXmlSchema()
        {
            var xUnitTestResultsWithInvalidStatus =
                @"<?xml version=""1.0"" encoding=""UTF-8""?>
                  <testsuites disabled="""" failures=""1"" tests=""2"" time=""2"">
                    <testsuite id=""0"" name=""TestSuite_1"" tests=""2"" failures=""1"" time=""2"">
                        <testcase name=""TestCase_1"" classname=""TestSuite_1"" time=""1"" status=""FAILED"">
                            <failure message=""My assert message!"" type=""BOOL"" />
                        </testcase>
                        <testcase name=""TestCase_2"" classname=""TestSuite_1"" time=""1"" status=""PAS""></testcase>
                    </testsuite>
                  </testsuites>";

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(xUnitTestResultsWithInvalidStatus));
            XUnitTestResultParser parser = new XUnitTestResultParser();

            Assert.ThrowsException<InvalidXUnitTestResultsException>(() => {
                IEnumerable<TestCaseResult> testResults = parser.Parse(ms);
            });
        }

        [TestMethod]
        public void TestParseTestResultsInvalidTimeXmlSchema()
        {
            var xUnitTestResultsWithInvalidStatus =
                @"<?xml version=""1.0"" encoding=""UTF-8""?>
                  <testsuites disabled="""" failures=""1"" tests=""2"" time=""abcdefg"">
                    <testsuite id=""0"" name=""TestSuite_1"" tests=""2"" failures=""1"" time=""2"">
                        <testcase name=""TestCase_1"" classname=""TestSuite_1"" time=""1"" status=""FAILED"">
                            <failure message=""My assert message!"" type=""BOOL"" />
                        </testcase>
                        <testcase name=""TestCase_2"" classname=""TestSuite_1"" time=""1"" status=""PASS""></testcase>
                    </testsuite>
                  </testsuites>";

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(xUnitTestResultsWithInvalidStatus));
            XUnitTestResultParser parser = new XUnitTestResultParser();

            Assert.ThrowsException<InvalidXUnitTestResultsException>(() => {
                IEnumerable<TestCaseResult> testResults = parser.Parse(ms);
            });
        }

        [TestMethod]
        public void TestParseTestResults()
        {
            var xUnitTestResults =
                @"<?xml version=""1.0"" encoding=""UTF-8""?>
                  <testsuites disabled="""" failures=""2"" tests=""10"" time=""20"">
                    <testsuite id=""0"" name=""TestSuite_1"" tests=""5"" failures=""1"" time=""10"">
                        <testcase name=""TestCase_1"" classname=""TestSuite_1"" time=""6"" status=""FAIL"">
                            <failure message=""My assert message!"" type=""BOOL"" />
                        </testcase>
                        <testcase name=""TestCase_2"" classname=""TestSuite_1"" time=""1"" status=""PASS""></testcase>
                        <testcase name=""TestCase_3"" classname=""TestSuite_1"" time=""1"" status=""PASS""></testcase>
                        <testcase name=""TestCase_4"" classname=""TestSuite_1"" time=""1"" status=""PASS""></testcase>
                        <testcase name=""TestCase_5"" classname=""TestSuite_1"" time=""1"" status=""PASS""></testcase>
                    </testsuite>
                    <testsuite id=""1"" name=""TestSuite_2"" tests=""5"" failures=""1"" time=""10"">
                        <testcase name=""TestCase_1"" classname=""TestSuite_2"" time=""6"" status=""FAIL"">
                            <failure message=""My assert message!"" type=""BOOL"" />
                        </testcase>
                        <testcase name=""TestCase_2"" classname=""TestSuite_2"" time=""1"" status=""PASS""></testcase>
                        <testcase name=""TestCase_3"" classname=""TestSuite_2"" time=""1"" status=""PASS""></testcase>
                        <testcase name=""TestCase_4"" classname=""TestSuite_2"" time=""1"" status=""PASS""></testcase>
                        <testcase name=""TestCase_5"" classname=""TestSuite_2"" time=""1"" status=""PASS""></testcase>
                    </testsuite>
                  </testsuites>";

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(xUnitTestResults));
            XUnitTestResultParser parser = new XUnitTestResultParser();

            IEnumerable<TestCaseResult> testResults = parser.Parse(ms);

            var firstTestCase = testResults.First();

            Assert.AreEqual(10, testResults.Count());
            Assert.AreEqual(2, testResults.Where(x => x.Outcome == TestOutcome.Failed).Count());
            Assert.AreEqual(8, testResults.Where(x => x.Outcome == TestOutcome.Passed).Count());
            
            Assert.AreEqual("TestCase_1", firstTestCase.Name);
            Assert.AreEqual("TestSuite_1", firstTestCase.TestSuiteName);
            Assert.AreEqual("TestSuite_1.TestCase_1", firstTestCase.FullyQualifiedName);
            Assert.AreEqual(TestOutcome.Failed, firstTestCase.Outcome);
            Assert.AreEqual("My assert message!", firstTestCase.ErrorMessage);
            Assert.AreEqual(TimeSpan.FromSeconds(6), firstTestCase.Duration);
        }

        [TestMethod]
        public void TestInvalidFilePath()
        {
            string filepath = "";
            XUnitTestResultParser parser = new XUnitTestResultParser();

            Assert.ThrowsException<FileNotFoundException>(() => {
                IEnumerable<TestCaseResult> testResults = parser.ParseFromFile(filepath);
            });
        }
    }
}
