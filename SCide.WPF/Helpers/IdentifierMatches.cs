using System.Collections;
using System.Collections.Generic;

namespace SCide.WPF.Helpers
{
    public class IdentifierMatches : IEnumerable<IdentifierMatch>
    {
        List<IdentifierMatch> _matches = null;

        public IdentifierMatches()
        {
            _matches = new List<IdentifierMatch>();
        }
        
        IEnumerator<IdentifierMatch> IEnumerable<IdentifierMatch>.GetEnumerator()
        {
            return this._matches.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._matches.GetEnumerator();
        }

        public void Add(IdentifierMatch m)
        {
            _matches.Add(m);
        }

        public void Clear()
        {
            _matches.Clear();
        }
    }
}
