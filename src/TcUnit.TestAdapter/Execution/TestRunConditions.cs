using System;
using System.Collections.Generic;
using System.Text;
using TcUnit.TestAdapter.Common;

namespace TcUnit.TestAdapter.Execution
{
    public class TestRunConditions
    {
        public string Target { get; set; }
        public string OperatingSystem { get; set; }
        public string BuildConfiguration { get; set; }
        public string TwinCATVersion { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
