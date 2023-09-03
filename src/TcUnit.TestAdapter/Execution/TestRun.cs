using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace TcUnit.TestAdapter.Execution
{
    public class TestRun
    {
        public IEnumerable<TestResult> Results { get; set; }
        public TestRunConditions Conditions { get; set; } = new TestRunConditions();
    }
}
