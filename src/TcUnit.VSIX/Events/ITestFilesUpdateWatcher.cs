using System;
using ProtractorTestAdapter.EventWatchers.EventArgs;

namespace ProtractorTestAdapter.EventWatchers
{
    public interface ITestFilesUpdateWatcher
    {
        event EventHandler<TestFileChangedEventArgs> FileChangedEvent;
        void AddWatch(string path);
        void RemoveWatch(string path);
    }
}