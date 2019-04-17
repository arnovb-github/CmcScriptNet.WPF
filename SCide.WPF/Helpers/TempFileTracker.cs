using System.Collections.Generic;
using System.Linq;

namespace SCide.WPF.Helpers
{
    internal static class TempFileTracker
    {
        private static IList<string> list = new List<string>();

        internal static void Add(string s)
        {
            list.Add(s);
        }

        internal static void DeleteAll()
        {
            foreach (string path in list)
            {
                try
                {
                    System.IO.File.Delete(path);
                }
                catch { } // swallow all errors
            }
        }

        internal static void Delete(string path)
        {
            if (list.SingleOrDefault(s => s.Equals(path)) != null)
            {
                try
                {
                    list.Remove(path);
                    System.IO.File.Delete(path);
                }
                catch { }
            }
        }

    }
}
