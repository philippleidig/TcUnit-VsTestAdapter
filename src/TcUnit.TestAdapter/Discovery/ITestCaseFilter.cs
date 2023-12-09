using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace TcUnit.TestAdapter.Discovery
{
    public interface ITestCaseFilter
    {
        bool MatchTestCase(TestCase testCase);
    }
}
