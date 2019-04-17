using System;
using System.Diagnostics;

namespace SCide.WPF.Commence
{
    public interface ICommenceMonitor
    {
        bool CommenceIsRunning { get; }
        Process Process { get; set; }
        void Focus(string databaseName);

        event EventHandler<EventArgs> CommenceProcessExited;
        event EventHandler<EventArgs> CommenceProcessStarted;
    }
}