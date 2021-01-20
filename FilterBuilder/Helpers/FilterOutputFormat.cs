using System.ComponentModel;

namespace FilterBuilder.Helpers
{
    public enum FilterOutputFormat
    {
        [Description("Raw (default)")]
        Raw, // quoted
        [Description("Escaped parameters")]
        Default // unquoted
    }
}