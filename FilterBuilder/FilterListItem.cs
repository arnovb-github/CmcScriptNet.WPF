using CmcScriptNet.FilterBuilder.Models;
using Vovin.CmcLibNet.Database;

namespace CmcScriptNet.FilterBuilder
{
    public class FilterListItem
    {
        public string DisplayName { get; set; }
        // we could get rid of the tag,
        // knowing that clausenumbers for actual filters are numbered 1-8,
        // the only remaining item would have 0
        public string Tag { get; set; } 
        public ICursorFilter Filter { get; set; }
        public int ClauseNumber { get; set; }
    }
}
