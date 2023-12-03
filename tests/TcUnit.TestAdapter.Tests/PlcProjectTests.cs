using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcUnit.TestAdapter.Models;

namespace TcUnit.TestAdapter.Tests
{
    [TestClass]
    public class PlcProjectTests
    {
        [TestMethod]
        public void TestParseProjectFromFile()
        {
            var filePath = @"PlcTestProject\FirstPLC\FirstPLC.plcproj";

            var plcProject = PlcProject.ParseFromProjectFile(filePath);

            Assert.IsTrue(plcProject.FunctionBlocks.Find(x => x.Name == "FB_TestSomething") != null);
            Assert.IsTrue(plcProject.ModuleClasses[0].DataTypes.Find(x => x.Name == "FB_TestSomething") != null);

            Assert.IsTrue(plcProject.References.Find(x => x.Name == "TcUnit") != null);
        }
    }
}
