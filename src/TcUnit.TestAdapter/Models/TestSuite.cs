
using System.Collections.Generic;

namespace TcUnit.TestAdapter.Models
{
    public class TestSuite
    {
        public string Name { get; set; }
        public List<TestMethod> Tests { get; set; } = new List<TestMethod>();

        private TestSuite()
        {

        }

        public static TestSuite ParseFromFunctionBlock(FunctionBlock_POU functionBlock)
        {
            var testSuite = new TestSuite();
            testSuite.Name = functionBlock.Name;

            foreach (var method in functionBlock.Methods)
            {
                var testName = method.Key;

                var test = new TestMethod();
                test.Name = testName;

                testSuite.Tests.Add(test);
            }

            return testSuite;
        }
    }
}
