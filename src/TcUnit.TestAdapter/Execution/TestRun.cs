using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;

namespace TcUnit.TestAdapter.Execution
{
    public class TestRun
    {
        public IEnumerable<TestResult> Results { get; set; }
        public TestRunContext Context { get; set; } = new TestRunContext();
    }
}
