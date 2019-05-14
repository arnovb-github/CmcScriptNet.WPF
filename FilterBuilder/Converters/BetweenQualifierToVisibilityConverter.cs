using System.Collections.Generic;
using System.Linq;
using Vovin.CmcLibNet.Database;

namespace CmcScriptNet.FilterBuilder.Converters
{
    public class BetweenQualifierToVisibilityConverter : BaseFilterQualifierToVisibilityConverter
    {
        public override IEnumerable<FilterQualifier> ApplicableQualifiers
        {
            get
            {
                return base.AllFilterQualifiers
                    .Where(w => w == FilterQualifier.Between);
            }
        }
    }
}