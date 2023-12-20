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
            var testFile = CreateTemporaryFile("TestFile.txt");

            var systemService = new SystemService(AmsNetId.LocalHost);

            Assert.IsTrue(systemService.FileExistsInBootFolder("TestFile.txt"));

            File.Delete(testFile);
        }

        [TestMethod]
        public void CleanUpBootFolder()
        {
            var testFile = CreateTemporaryFile("TestFile.txt");

            var systemService = new SystemService(AmsNetId.LocalHost);
            var targetInfo = systemService.GetDeviceInfo();
            systemService.CleanUpBootDirectory(targetInfo.ImageOsName);

            Assert.IsFalse(systemService.FileExistsInBootFolder("TestFile.txt"));
        }

        private string CreateTemporaryFile(string name)
        {
            var bootDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Beckhoff\TwinCAT3\3.1", "BootDir", null);

            if (bootDir == null)
                Assert.Fail("BootDir not found in registry");

            var testFilePath = Path.Combine(bootDir.ToString(), name);
            File.WriteAllText(testFilePath, "Test");

            return testFilePath;
        }
    }
}
