using Vovin.CmcLibNet.Database;

namespace CmcScriptNet.FilterBuilder.Helpers
{
    // for use in qualifier comboboxes
    public class QualifierMember
    {
        public string Description { get; set; }
        public FilterQualifier Value { get; set; }
        public int FieldValues { get; set; }
    }
}
