using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace TcUnit.TestAdapter.Discovery
{
    public interface ITestCaseFilter
    {
        bool MatchTestCase(TestCase testCase);
    }
}
