using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace TcUnit.TestAdapter.Tests.Mocks
{
    internal class FrameworkHandleMock : IFrameworkHandle
    {
        public List<TestResult> TestResults = new List<TestResult>();

        public bool EnableShutdownAfterTestRun
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public int LaunchProcessWithDebuggerAttached(string filePath, string? workingDirectory, string? arguments, IDictionary<string, string?>? environmentVariables)
        {
            throw new NotImplementedException();
        }

        public void RecordAttachments(IList<AttachmentSet> attachmentSets)
        {
            throw new NotImplementedException();
        }

        public void RecordEnd(TestCase testCase, TestOutcome outcome)
        {
            
        }

        public void RecordResult(TestResult testResult)
        {
            TestResults.Add(testResult);
        }

        public void RecordStart(TestCase testCase)
        {
            
        }

        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            
        }
    }
}
