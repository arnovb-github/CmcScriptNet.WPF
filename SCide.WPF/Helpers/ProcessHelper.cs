using SCide.WPF.Extensions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SCide.WPF.Helpers
{
    public static class ProcessHelper
    {
        public static int GetProcessCountInCurrentSession(string processName)
        {
            Process[] ps = Process.GetProcessesByName(processName);
            int currentSessionID = Process.GetCurrentProcess().SessionId;
            Process[] inSameSession = (from c in ps where c.SessionId == currentSessionID select c).ToArray();
            return inSameSession.Length;
        }

        /// <summary>
        /// Returns the first process with the given process name.
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        /// <remarks>Caution: this may NOT be the instance that COM will bind to!</remarks>
        public static Process GetProcessToMonitor(string processName)
        {
            Process[] ps = Process.GetProcessesByName(processName);

            if (ps.Length > 0)
            {
                // we should determine if the process is run with elevated permissions
                // if the elevation level of this assembly and the commence process do not match,
                // an error occurs
                // we should first check our own elevation level
                // then see if it matches any commence process
                bool elevated = Process.GetCurrentProcess().IsProcessElevated();
                return ps.FirstOrDefault(p => p.IsProcessElevated() == elevated);
                //return ps.First();
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
