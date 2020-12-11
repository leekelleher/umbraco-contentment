/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class NumberInputValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(NumberInputDataEditor.DataEditorAlias);

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(int);

        // TODO: [LK:2020-12-11] Commented out the value-type feature for the time being. Adds additional complexity that I don't currently need.
        //public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        //{
        //    if (propertyType.DataType.Configuration is Dictionary<string, object> config && config.TryGetValue(UmbConfigurationKeys.DataValueType, out var tmp) && tmp is string valueType)
        //    {
        //        switch (valueType)
        //        {
        //            case ValueTypes.Decimal:
        //                return typeof(decimal);

        //            case ValueTypes.Integer:
        //                return typeof(int);

        //            default:
        //                break;
        //        }
        //    }

        //    return base.GetPropertyValueType(propertyType);
        //}

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null)
            {
                return base.ConvertSourceToIntermediate(owner, propertyType, source, preview);
            }

            var type = GetPropertyValueType(propertyType);
            return source.TryConvertTo(type).ResultOr(type.GetDefaultValue());
        }
    }
}
