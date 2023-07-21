using System.Collections.Generic;
using TcUnit.Core.Models;

namespace TcUnit.Core
{
    public interface ITestRunner
    {
        IEnumerable<TestCase> DiscoverTests(IEnumerable<string> sources);

        void RunTests(IEnumerable<string> sources);
        void RunTests(IEnumerable<TestCase> tests);
    }
}