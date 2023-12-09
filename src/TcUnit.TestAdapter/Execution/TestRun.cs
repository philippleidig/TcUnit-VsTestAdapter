using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;

namespace TcUnit.TestAdapter.Execution
{
    public class TestRun
    {
        public IEnumerable<TestResult> Results { get; set; }
        public TestRunConditions Conditions { get; set; } = new TestRunConditions();
    }
}
