using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;

namespace TcUnit.TestAdapter.Execution
{
    public class TestCaseResult
    {
        public string TestSuiteName { get; set; }
        public string FullyQualifiedName { get; set; }
        public string Name { get; set; }
        public TestOutcome Outcome { get; set; }
        public string ErrorMessage { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
