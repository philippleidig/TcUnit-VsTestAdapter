using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcUnit.Core.Models;

namespace TcUnit.Core
{
    public class TestSuite
    {
        private readonly List<TestCase> _testCases;
        public IEnumerable<TestCase> TestCases => _testCases;
    }
}
