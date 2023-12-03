using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TcUnit.TestAdapter.Tests
{
    [TestClass]
    public class TwinCATModuleClassTests
    {
        [TestMethod]
        public void TestParseFromFile() {
            var filePath = @"PlcTestProject\FirstPLC\FirstPLC.tmc";

            var tmc = Models.TwinCATModuleClass.ParseFromFilePath(filePath);

            var testSuiteDatatypes = tmc.DataTypes.Find(x => x.ExtendsType == TestAdapter.TestSuiteBaseClass);

            Assert.IsTrue(testSuiteDatatypes != null);

            var testSuiteSymbol = tmc.Modules[0].DataAreas[1].Symbols.Find(x => x.BaseType == "FB_TestSuite1");
            Assert.IsNotNull(testSuiteSymbol);
            Assert.AreEqual("PRG_TESTS.fbTestSuite1Instance1", testSuiteSymbol.Name);
        }
    }
}
