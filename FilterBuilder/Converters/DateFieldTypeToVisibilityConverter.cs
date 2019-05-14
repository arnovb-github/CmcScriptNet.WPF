using System.Collections.Generic;
using Vovin.CmcLibNet.Database;

namespace CmcScriptNet.FilterBuilder.Converters
{
    public class DateFieldTypeToVisibilityConverter : BaseFieldTypeToVisibilityConverter
    {
        public override IEnumerable<CommenceFieldType> FieldTypes => new List<CommenceFieldType>()
        {
            CommenceFieldType.Date
        };
    }
}
