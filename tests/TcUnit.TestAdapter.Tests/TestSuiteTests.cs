using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcUnit.TestAdapter.Models
{
    [TestClass]
    public class TestSuiteTests
    {
        [TestMethod]
        public void TestParseFromFile() {
            var filePath = @"PlcTestProject\FirstPLC\POUs\FB_TestSuite2.TcPOU";

            var pou = FunctionBlock_POU.Load(filePath);

            var testSuite = TestSuite.ParseFromFunctionBlock(pou);

            Assert.IsTrue(testSuite.Tests.Find(x => x.Name == "TestCase2A") != null);
            Assert.IsTrue(testSuite.Tests.Find(x => x.Name == "TestCase2B") != null);
            Assert.IsTrue(testSuite.Tests.Find(x => x.Name == "TestCase2C") != null);
        }
    }
}
