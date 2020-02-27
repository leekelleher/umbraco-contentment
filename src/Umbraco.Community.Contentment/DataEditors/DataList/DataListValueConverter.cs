/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class DataListValueConverter : PropertyValueConverterBase
    {
        private readonly ConfigurationEditorUtility _utility;

        public DataListValueConverter(ConfigurationEditorUtility utility)
            : base()
        {
            _utility = utility;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(DataListDataEditor.DataEditorAlias);

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            if (propertyType.DataType.Configuration is Dictionary<string, object> configuration &&
                configuration.TryGetValue(DataListConfigurationEditor.ListEditor, out var tmp2) &&
                tmp2 is JArray array2 && array2.Count > 0)
            {
                var editor = _utility.GetConfigurationEditor<IDataListEditor>(array2[0].Value<string>("type"));

                return editor?.HasMultipleValues == true
                    ? typeof(IEnumerable<string>)
                    : typeof(string);
            }

            return typeof(string);
        }

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            if (source is string value)
            {
                if (value.DetectIsJson() == false)
                {
                    return value;
                }

                var items = JsonConvert.DeserializeObject<IEnumerable<string>>(value);

                if (propertyType.ClrType.IsGenericType)
                {
                    return items;
                }

                return string.Join(",", items);
            }

            return base.ConvertSourceToIntermediate(owner, propertyType, source, preview);
        }
    }
}
