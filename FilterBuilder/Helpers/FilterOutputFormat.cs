using System.ComponentModel;

namespace FilterBuilder.Helpers
{
    public enum FilterOutputFormat
    {
        [Description("Quote arguments (default)")]
        Default, // quoted
        [Description("Raw")]
        Raw // unquoted
    }
}