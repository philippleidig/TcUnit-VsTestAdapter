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

            var testSuiteDatatypes = tmc.DataTypes.Find(x => x.ExtendsType == "FB_TestSuite");

            Assert.IsTrue(testSuiteDatatypes != null);
        }
    }
}
