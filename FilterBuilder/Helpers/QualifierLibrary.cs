using System.Collections.Generic;
using System.Linq;
using Vovin.CmcLibNet.Database;

namespace CmcScriptNet.FilterBuilder.Helpers
{
    internal static class QualifierLibrary
    {
        /// <summary>
        /// Returns the valid qualifiermembers for a given Commence field-type
        /// </summary>
        internal static IList<QualifierMember> GetQualifierMembers
            (Dictionary<FilterQualifier, QualifierMember>  dict, CommenceFieldType fieldType)
        {
            switch (fieldType)
            {
                // note the absence of HasDuplicates for Name
                // you can't check duplicates in a dde filter!
                case CommenceFieldType.Name:
                case CommenceFieldType.Text:
                case CommenceFieldType.Email:
                case CommenceFieldType.URL:
                default:
                    return dict.Where(w => w.Key.Equals(FilterQualifier.Contains)
                            || w.Key.Equals(FilterQualifier.DoesNotContain)
                            || w.Key.Equals(FilterQualifier.EqualTo)
                            || w.Key.Equals(FilterQualifier.NotEqualTo)
                            || w.Key.Equals(FilterQualifier.Between)
                            || w.Key.Equals(FilterQualifier.Blank))
                        .Select(s => s.Value).ToList();
                case CommenceFieldType.Date:
                    return dict.Where(w => w.Key.Equals(FilterQualifier.On)
                            || w.Key.Equals(FilterQualifier.After)
                            || w.Key.Equals(FilterQualifier.Before)
                            || w.Key.Equals(FilterQualifier.Between)
                            || w.Key.Equals(FilterQualifier.Blank))
                        .Select(s => s.Value).ToList();

                case CommenceFieldType.Time:
                    return dict.Where(w => w.Key.Equals(FilterQualifier.At)
                            || w.Key.Equals(FilterQualifier.After)
                            || w.Key.Equals(FilterQualifier.Before)
                            || w.Key.Equals(FilterQualifier.Between)
                            || w.Key.Equals(FilterQualifier.Blank))
                        .Select(s => s.Value).ToList();

                case CommenceFieldType.Checkbox:
                    return dict.Where(w => w.Key.Equals(FilterQualifier.Checked)
                            || w.Key.Equals(FilterQualifier.NotChecked))
                        .Select(s => s.Value).ToList();

                case CommenceFieldType.Telephone:
                    return dict.Where(w => w.Key.Equals(FilterQualifier.Contains)
                            || w.Key.Equals(FilterQualifier.DoesNotContain)
                            || w.Key.Equals(FilterQualifier.EqualTo)
                            || w.Key.Equals(FilterQualifier.Blank))
                        .Select(s => s.Value).ToList();

                case CommenceFieldType.Number:
                case CommenceFieldType.Calculation:
                    return dict.Where(w => w.Key.Equals(FilterQualifier.EqualTo)
                            || w.Key.Equals(FilterQualifier.NotEqualTo)
                            || w.Key.Equals(FilterQualifier.GreaterThan)
                            || w.Key.Equals(FilterQualifier.LessThan)
                            || w.Key.Equals(FilterQualifier.Between))
                        .Select(s => s.Value).ToList();

                case CommenceFieldType.Selection:
                    return dict.Where(w => w.Key.Equals(FilterQualifier.EqualTo)
                            || w.Key.Equals(FilterQualifier.NotEqualTo))
                        .Select(s => s.Value).ToList();
            }
        }
    }
}
