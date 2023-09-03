using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Xsl;
using TcUnit.TestAdapter.Execution;
using TcUnit.TestAdapter.RunSettings;

namespace TcUnit.TestAdapter.Abstractions
{
    public interface ITestRunner
    {
        IEnumerable<TestCase> DiscoverTests(string source);

        TestRun RunTests(string source, IEnumerable<TestCase> tests, TestSettings runSettings);
    }
}
