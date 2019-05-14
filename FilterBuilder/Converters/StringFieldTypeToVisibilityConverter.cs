using System.Collections.Generic;
using Vovin.CmcLibNet.Database;

namespace CmcScriptNet.FilterBuilder.Converters
{
    public class StringFieldTypeToVisibilityConverter : BaseFieldTypeToVisibilityConverter
    {
        public override IEnumerable<CommenceFieldType> FieldTypes => new List<CommenceFieldType>()
        {
            CommenceFieldType.Email,
            CommenceFieldType.Name,
            CommenceFieldType.Telephone,
            CommenceFieldType.Text,
            CommenceFieldType.URL
        };
   }
}
