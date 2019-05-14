using Vovin.CmcLibNet.Database;

namespace CmcScriptNet.FilterBuilder.Models
{
    public class FieldListItem
    {
        public string FieldName { get; set; }
        public ICommenceFieldDefinition FieldDefinition { get; set; }
        public string ConnectionName { get; set; }
        public string ToCategory { get; set; }
        public string DisplayName { get; set; }
        // convenience property
        public bool IsField => ToCategory == null;
    }
}