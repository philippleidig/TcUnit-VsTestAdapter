using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TcUnit.TestAdapter.Models
{
    public class TestSuite
    {
        public string Name { get; set; }
        public List<TestMethod> Tests { get; set; } = new List<TestMethod>();

        private TestSuite()
        {

        }

        private void ParseTestCases(StructuredTextImplementation implementation)
        {
            // match all groups in multi-line string where there is a section starting with
            // TEST('name') OR TEST_ORDERED('name') and ending with TEST_FINISHED();

            /*
            TEST('TestCase1B');

            AssertTrue(TRUE, 'Condition is not true');

            TEST_FINISHED();

            TEST('TestCase1A');

            AssertTrue(2+2=4, 'Condition is not true');

            TEST_FINISHED();

            */

            // use regex to find all test cases, for example above there would be two test cases 'TestCase1B' and 'TestCase1A'

            var regex = new Regex(@"(?<=TEST(_ORDERED)?\()'(?<testName>[^']+)'\)(.*?)(?=TEST_FINISHED)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            var matches = regex.Matches(implementation.ToString());
            foreach (Match match in matches)
            {
                var testName = match.Groups["testName"].Value;
                var test = new TestMethod();
                test.Name = testName;

                Tests.Add(test);
            }
        }

        public static TestSuite ParseFromFunctionBlock(FunctionBlock_POU functionBlock)
        {
            var testSuite = new TestSuite();
            testSuite.Name = functionBlock.Name;

            if(functionBlock.Implementation != null)
            {
                testSuite.ParseTestCases(functionBlock.Implementation);
            }
            
            foreach (var method in functionBlock.Methods.Values)
            {
                testSuite.ParseTestCases(method.Implementation);
            }

            return testSuite;
        }
    }
}
