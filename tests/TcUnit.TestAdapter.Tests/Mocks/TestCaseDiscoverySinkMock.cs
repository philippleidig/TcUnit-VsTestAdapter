using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcUnit.TestAdapter.Tests.Mocks
{
    internal class TestCaseDiscoverySinkMock : ITestCaseDiscoverySink
    {
        public List<TestCase> TestCases = new List<TestCase>();
        public void SendTestCase(TestCase discoveredTest)
        {
            TestCases.Add(discoveredTest);
        }
    }
}
