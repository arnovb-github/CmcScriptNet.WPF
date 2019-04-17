using System.Diagnostics;
using System.Linq;

namespace SCide.WPF.Helpers
{
    public static class ProcessHelper
    {
        public static int GetProcessCountInCurrentSession(string processName)
        {
            Process[] ps = Process.GetProcessesByName(processName);
            int currentSessionID = Process.GetCurrentProcess().SessionId;
            Process[] sameAsthisSession = (from c in ps where c.SessionId == currentSessionID select c).ToArray();
            return sameAsthisSession.Length;
        }

        /// <summary>
        /// Returns the first process with the given process name.
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        /// <remarks>This may NOT be the instance that COM will bind to!</remarks>
        public static Process GetProcessToMonitor(string processName)
        {
            Process[] ps = Process.GetProcessesByName(processName);
            if (ps.Length > 0)
            {
                return ps.First();
            }
            return null;
        }

        public static Process GetProcessFromTitle(string processName, string windowTitle)
        {
            Process[] ps = Process.GetProcessesByName(processName).ToArray();
            foreach (Process p in ps)
            {
                if (p.MainWindowTitle.Contains(windowTitle))
                {
                    return p;
                }
            }
            return null;
        }
    }
}
