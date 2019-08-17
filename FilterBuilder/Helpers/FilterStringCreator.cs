using System;
using System.Text;
using Vovin.CmcLibNet.Database;

namespace FilterBuilder.Helpers
{
    internal static class FilterStringCreator
    {
        internal static string ToString(ICursorFilter filter, FilterOutputFormat format)
        {
            switch (format)
            {
                case FilterOutputFormat.Raw:
                    var d = GetDelegate(filter.FiltertypeIdentifier);
                    if (d != null)
                    {
                        return filter.ToString(d);
                    }
                    else
                        return "Unknown filtertype";
                default:
                    return filter.ToString();
            }
        }

        private static Func<ICursorFilter, string> GetDelegate(string filterType)
        {
            switch (filterType.ToLower())
            {
                case "f":
                    return FormatFFilterRaw;
                case "cti":
                    return FormatCTIFilterRaw;
                case "ctcti":
                    return FormatCTCTIFilterRaw;
                case "ctcf":
                    return FormatCTCFFilterRaw;
                default:
                    return null;
            }
        }

        private static string GetClarifiedItemName(string itemName, string clarifySeparator, string clarifyValue)
        {
            if (string.IsNullOrEmpty(itemName)) { return string.Empty; }
            if (!string.IsNullOrEmpty(clarifySeparator)) // connection specified as clarified
            {
                return itemName.PadRight(50) + clarifySeparator + clarifyValue.PadRight(40);
            }
            else // connection not specified as clarified
            {
                return itemName;
            }
        }

        private static string FormatCTIFilterRaw(ICursorFilter filter)
        {
            ICursorFilterTypeCTI f = (CursorFilterTypeCTI)filter;
            StringBuilder sb = new StringBuilder("[ViewFilter(");
            sb.Append(f.ClauseNumber.ToString());
            sb.Append(',');
            sb.Append(f.FiltertypeIdentifier);
            sb.Append(',');
            sb.Append(f.Except ? "NOT," : ",");
            sb.Append(f.Connection);
            sb.Append(',');
            sb.Append(f.Category);
            sb.Append(',');
            sb.Append(GetClarifiedItemName(f.Item, f.ClarifySeparator, f.ClarifyValue));
            sb.Append(")]");
            return sb.ToString();
        }


        private static string FormatFFilterRaw(ICursorFilter filter)
        {
            ICursorFilterTypeF f = (ICursorFilterTypeF)filter;
            StringBuilder sb = new StringBuilder("[ViewFilter(");
            sb.Append(f.ClauseNumber.ToString());
            sb.Append(',');
            sb.Append(f.FiltertypeIdentifier);
            sb.Append(',');
            sb.Append((f.Except) ? "NOT," : ",");
            if (f.SharedOptionSet)
            {
                sb.Append(',');
                sb.Append((f.Shared) ? "Shared" : "Local");
                sb.Append(",,"); // two!
            }
            else
            {
                sb.Append(f.FieldName);
                sb.Append(',');
                sb.Append(f.QualifierString);
                sb.Append(',');
                if (f.Qualifier == FilterQualifier.Between) // TODO code smell, this value may not be set by COM clients
                {
                    sb.Append(f.FilterBetweenStartValue);
                    sb.Append(',');
                    sb.Append(f.FilterBetweenEndValue);
                }
                else
                {
                    sb.Append(f.FieldValue);
                    sb.Append(',');
                    sb.Append((f.MatchCase) ? "1" : "0");
                }
            }
            sb.Append(")]");
            return sb.ToString();
        }

        private static string FormatCTCFFilterRaw(ICursorFilter filter)
        {
            ICursorFilterTypeCTCF f = (CursorFilterTypeCTCF)filter;
            StringBuilder sb = new StringBuilder("[ViewFilter(");
            sb.Append(f.ClauseNumber.ToString());
            sb.Append(',');
            sb.Append(f.FiltertypeIdentifier);
            sb.Append(',');
            sb.Append(f.Except ? "NOT," : ",");
            sb.Append(f.Connection);
            sb.Append(',');
            sb.Append(f.Category);
            sb.Append(',');
            if (f.SharedOptionSet)
            {
                sb.Append(',');
                sb.Append((f.Shared) ? "Shared" : "Local");
                sb.Append(",,"); // two!
            }
            else
            {
                sb.Append(f.FieldName);
                sb.Append(',');
                sb.Append(f.QualifierString);
                sb.Append(',');
                if (f.Qualifier == FilterQualifier.Between)  // TODO code smell, this value may not be set by COM clients
                {
                    sb.Append(f.FilterBetweenStartValue);
                    sb.Append(',');
                    sb.Append(f.FilterBetweenEndValue);
                }
                else
                {
                    sb.Append(f.FieldValue);
                    sb.Append(',');
                    sb.Append((f.MatchCase) ? "1" : "0");
                }
            }
            sb.Append(")]");
            return sb.ToString();
        }

        private static string FormatCTCTIFilterRaw(ICursorFilter filter)
        {
            ICursorFilterTypeCTCTI f = (CursorFilterTypeCTCTI)filter;
            StringBuilder sb = new StringBuilder("[ViewFilter(");
            sb.Append(f.ClauseNumber.ToString());
            sb.Append(',');
            sb.Append(f.FiltertypeIdentifier);
            sb.Append(',');
            sb.Append((f.Except) ? "NOT" : ",");
            sb.Append(f.Connection);
            sb.Append(',');
            sb.Append(f.Category);
            sb.Append(',');
            sb.Append(f.Connection2);
            sb.Append(',');
            sb.Append(f.Category2);
            sb.Append(',');
            sb.Append(GetClarifiedItemName(f.Item, f.ClarifySeparator, f.ClarifyValue));
            sb.Append(")]");
            return sb.ToString();
        }
    }
}

