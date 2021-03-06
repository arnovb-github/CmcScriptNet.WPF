﻿using System.Collections.Generic;
using Vovin.CmcLibNet.Database;

namespace CmcScriptNet.FilterBuilder.Converters
{
    public class CheckBoxFieldTypeToVisibilityConverter : BaseFieldTypeToVisibilityConverter
    {
        public override IEnumerable<CommenceFieldType> FieldTypes { get; } = new List<CommenceFieldType>()
        {
            CommenceFieldType.Checkbox
        };
    }
}
