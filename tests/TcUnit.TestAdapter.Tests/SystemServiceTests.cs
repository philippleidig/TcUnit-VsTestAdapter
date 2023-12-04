using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace TcUnit.TestAdapter.Models
{
    [TestClass]
    public class SystemServiceTests
    {
        [TestMethod]
        public void SystemServiceConstructorTests()
        {
            var systemService = new SystemService(AmsNetId.LocalHost);

            var targetInfo = systemService.GetDeviceInfo();
            Assert.IsNotNull(targetInfo);
            Assert.AreEqual(targetInfo.RTPlatform, Common.RTPlatform.WinNT);
        }

        [TestMethod]
        public void FileExistsTests()
        {
            var systemService = new SystemService(AmsNetId.LocalHost);

            // current assembly location
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Assert.IsTrue(systemService.FileExists(assemblyLocation));
        }

        [TestMethod]
        public void FileExistsInBootFolderTests()
        {
            var systemService = new SystemService(AmsNetId.LocalHost);

            Assert.IsTrue(systemService.FileExistsInBootFolder("CurrentConfig.xml"));
        }

        [TestMethod]
        public void CleanUpBootFolder()
        {
            var systemService = new SystemService(AmsNetId.LocalHost);
            var targetInfo = systemService.GetDeviceInfo();
            systemService.CleanUpBootDirectory(targetInfo.ImageOsName);

            Assert.IsFalse(systemService.FileExistsInBootFolder("CurrentConfig.xml"));
        }
    }
}
