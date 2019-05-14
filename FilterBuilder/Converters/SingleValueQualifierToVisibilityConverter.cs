using System.Collections.Generic;
using System.Linq;
using Vovin.CmcLibNet.Database;

namespace CmcScriptNet.FilterBuilder.Converters
{
    public class SingleValueQualifierToVisibilityConverter : BaseFilterQualifierToVisibilityConverter
    {
        public override IEnumerable<FilterQualifier> ApplicableQualifiers
        {
            // we could change this to use the FilterValuesAttribute of a FilterQualifier
            get
            {
                return base.AllFilterQualifiers
                    .Where(w => w != FilterQualifier.Between && w != FilterQualifier.Blank);
            }
        }
    }
}
