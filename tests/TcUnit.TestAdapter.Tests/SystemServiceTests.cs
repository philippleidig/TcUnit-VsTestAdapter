using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcUnit.TestAdapter.Models
{
    [TestClass]
    public class SystemServiceTests
    {
        [TestMethod]
        public void SystemServiceConstructorTests()
        {
            var systemService = new SystemService("86.144.205.105.1.1");

            var targetInfo = systemService.GetDeviceInfo();
            Assert.IsNotNull(targetInfo);
        }
    }
}
