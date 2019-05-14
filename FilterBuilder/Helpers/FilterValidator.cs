using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmcScriptNet.FilterBuilder.Extensions;
using Vovin.CmcLibNet.Database;

namespace CmcScriptNet.FilterBuilder.Helpers
{
    /// <summary>
    /// Validates a filter
    /// </summary>
    /// <remarks>Checks if the filter has the required, settable parameters set.
    /// That does NOT mean the syntax is valid.</remarks>
    internal class FilterValidator
    {
        private readonly ICursorFilter _filter;

        public FilterValidator(ICursorFilter filter)
        {
            _filter = filter;
        }

        internal bool Validate()
        {
            switch (_filter)
            {
                case CursorFilterTypeF _filter:
                    return ValidateFFilter(_filter);

                case CursorFilterTypeCTI _filter:
                    return ValidateCTIFilter(_filter);

                case CursorFilterTypeCTCF _filter:
                    return ValidateCTCFFilter(_filter);

                case CursorFilterTypeCTCTI _filter:
                    return ValidateCTCTIFilter(_filter);
            }
            return false;
        }

        private bool ValidateCTCTIFilter(CursorFilterTypeCTCTI f)
        {
            return (!string.IsNullOrEmpty(f.Connection)
                && !string.IsNullOrEmpty(f.Category)
                && !string.IsNullOrEmpty(f.Connection2)
                && !string.IsNullOrEmpty(f.Category2)
                && !string.IsNullOrEmpty(f.Item));
        }

        private bool ValidateCTCFFilter(CursorFilterTypeCTCF f)
        {
            if (string.IsNullOrEmpty(f.Connection)
                || string.IsNullOrEmpty(f.Category)
                || string.IsNullOrEmpty(f.FieldName))
            {
                return false;
            }

            if (f.Qualifier.GetAttribute<FilterValuesAttribute>()?.Number == 2)
            {
                return (!string.IsNullOrEmpty(f.FilterBetweenStartValue)
                    && !string.IsNullOrEmpty(f.FilterBetweenEndValue));
            }

            if (f.Qualifier.GetAttribute<FilterValuesAttribute>()?.Number == 1)
            {
                return (!string.IsNullOrEmpty(f.FieldValue));
            }
            return false;
        }

        private bool ValidateCTIFilter(CursorFilterTypeCTI f)
        {
            return (!string.IsNullOrEmpty(f.Connection)
                && !string.IsNullOrEmpty(f.Category)
                && !string.IsNullOrEmpty(f.Item));
        }

        private bool ValidateFFilter(CursorFilterTypeF f)
        {
            if (string.IsNullOrEmpty(f.FieldName)) { return false; }

            if (f.Qualifier.GetAttribute<FilterValuesAttribute>()?.Number == 2)
            {
                return (!string.IsNullOrEmpty(f.FilterBetweenStartValue)
                    && !string.IsNullOrEmpty(f.FilterBetweenEndValue));
            }

            if (f.Qualifier.GetAttribute<FilterValuesAttribute>()?.Number == 1)
            {
                return (!string.IsNullOrEmpty(f.FieldValue));
            }
            return false;
        }
    }
}
