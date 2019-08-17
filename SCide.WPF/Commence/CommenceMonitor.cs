using SCide.WPF.Helpers;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;

namespace SCide.WPF.Commence
{
    public class CommenceMonitor : ICommenceMonitor
    {
        #region Events
        public event EventHandler<EventArgs> CommenceProcessExited;
        public event EventHandler<EventArgs> CommenceProcessStarted;
        #endregion

        #region Fields        
        private Process _process;
        private const string COMMENCE_PROCESS = "commence";
        #endregion

        #region Constructors
        public CommenceMonitor()
        {
            _process = GetProcess(COMMENCE_PROCESS);
            if (_process != null)
            {
                Process = _process;
                CommenceIsRunning = true;
            }
            else
            {
                StartMonitoring(COMMENCE_PROCESS, 1000);
            }
        }
        #endregion

        #region Properties
        public Process Process
        {
            get
            {
                return _process;
            }
            set
            {
                _process = value;
                _process.EnableRaisingEvents = true;
                _process.Exited += Process_Exited;
            }
        }

        public bool CommenceIsRunning { get; set; }
        #endregion

        #region Event Handlers
        private void Process_Exited(object sender, EventArgs e)
        {
            CommenceIsRunning = false;
            _process.Exited -= Process_Exited;
            CommenceProcessExited?.Invoke(this, new EventArgs());
            StartMonitoring(COMMENCE_PROCESS, 1000);
        }
        #endregion

        #region Methods
        // will poll the system every x milliseconds for the existence of a process
        private System.Timers.Timer _timer = null;
        private volatile bool _requestStop = false;
        private void StartMonitoring(string processName, double interval)
        {
            _timer = new System.Timers.Timer();
            _requestStop = false;
            _timer.Interval = interval;
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = false;
            _timer.Start();
        }

        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // do work....
            Process process = GetProcess(COMMENCE_PROCESS);
            process.e
            if (process != null)
            {
                // now we run into an interesting problem:
                // Commence does not have to have a database opened at this time,
                // for instance when the /prompt option is used when starting Commence
                // we have to wait until it has...
                process.WaitForInputIdle();
                while (process.MainWindowHandle == IntPtr.Zero)
                {
                    if (process.HasExited)
                    {
                        goto continue_as_if_nothing_happened;
                    }
                    // should we include some kind of timeout here?
                }
                process.WaitForInputIdle();
                this.CommenceIsRunning = true;
                this.Process = process;
                Stop(); // is this needed? AutoReset is set to false anway.
                Thread.Sleep(1000); // wait a second
                CommenceProcessStarted?.Invoke(this, new EventArgs()); // signal assembly we're good to go
            }

            continue_as_if_nothing_happened:
            // issue stop or restart
            if (!_requestStop)
            {
                _timer.Start();//restart the timer
            }
        }

        private void Stop()
        {
            _requestStop = true;
            _timer.Stop();
        }

        private void Start()
        {
            _requestStop = false;
            _timer.Start();
        }

        private Process GetProcess(string processName)
        {
            Process process;
            switch (ProcessHelper.GetProcessCountInCurrentSession(processName))
            {
                case 0:
                    return null;
                case 1:
                    return process = ProcessHelper.GetProcessToMonitor(processName);
                default:
                    return process = ProcessHelper.GetProcessToMonitor(processName);
            }
        }

        // TODO this hangs the application if Commence doesnt have a main window
        public void Focus(string databaseName)
        {
            Process process = ProcessHelper.GetProcessFromTitle(COMMENCE_PROCESS, databaseName);
            if (process != null)
                {
                AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
                if (element != null)
                {
                    element.SetFocus();
                    var pattern = (WindowPattern)element.GetCurrentPattern(WindowPattern.Pattern);
                    pattern.SetWindowVisualState(WindowVisualState.Normal);
                }
            }
        }
        #endregion
    }
}
