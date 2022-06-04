/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using UmbConstants = Umbraco.Cms.Core.Constants;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class TemplatedLabelValueConverter : LabelValueConverter
    {
        // NOTE: I wanted to reuse the `LabelValueConverter` code, but it's tightly-coupled to `LabelConfiguration`, so can't reuse it here. ಥ_ಥ [LK:2021-07-12]
        // https://github.com/umbraco/Umbraco-CMS/blob/release-8.14.0/src/Umbraco.Core/PropertyEditors/ValueConverters/LabelValueConverter.cs

        private readonly Type _defaultObjectType = typeof(string);

        private readonly Dictionary<string, Type> _objectTypes = new Dictionary<string, Type>
        {
            { ValueTypes.Date, typeof(DateTime) },
            { ValueTypes.DateTime, typeof(DateTime) },
            { ValueTypes.Time, typeof(TimeSpan) },
            { ValueTypes.Decimal, typeof(decimal) },
            { ValueTypes.Integer, typeof(int) },
            { ValueTypes.Bigint, typeof(long) },
        };

        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(TemplatedLabelDataEditor.DataEditorAlias);

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            if (propertyType.DataType.Configuration is Dictionary<string, object> config &&
                config.TryGetValueAs(UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType, out string valueType) == true &&
                ValueTypes.IsValue(valueType) == true)
            {
                return _objectTypes.ContainsKey(valueType) == true
                    ? _objectTypes[valueType]
                    : _defaultObjectType;
            }

            return _defaultObjectType;
        }

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            var valueType = GetPropertyValueType(propertyType);
            return source.TryConvertTo(valueType).ResultOr(valueType.GetDefaultValue());
        }
    }
}
