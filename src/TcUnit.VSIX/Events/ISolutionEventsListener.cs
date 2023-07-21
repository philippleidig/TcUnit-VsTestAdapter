using System;
using ProtractorTestAdapter.EventWatchers.EventArgs;

namespace ProtractorTestAdapter.EventWatchers
{
    public interface ISolutionEventsListener
    {
        /// <summary>
        /// Fires an event when a project is opened/closed/loaded/unloaded
        /// </summary>
        event EventHandler<SolutionEventsListenerEventArgs> SolutionProjectChanged;

        void StartListeningForChanges();
        void StopListeningForChanges();
        event EventHandler SolutionUnloaded;
    }
}