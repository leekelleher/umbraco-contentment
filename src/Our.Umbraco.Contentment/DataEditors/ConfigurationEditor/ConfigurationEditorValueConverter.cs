/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Serialization;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class ConfigurationEditorValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(PublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(ConfigurationEditorDataEditor.DataEditorAlias);

        public override PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType) => PropertyCacheLevel.Element;

        public override Type GetPropertyValueType(PublishedPropertyType propertyType) => typeof(IEnumerable<IConfigurationEditorItem>);

        public override object ConvertSourceToIntermediate(IPublishedElement owner, PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source is string value)
            {
                return JArray.Parse(value);
            }

            return base.ConvertSourceToIntermediate(owner, propertyType, source, preview);
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, PublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if (inter is JArray array)
            {
                var items = new List<IConfigurationEditorItem>();

                foreach (var item in array)
                {
                    // NOTE: Using `TypeFinder` here, as `TypeLoader` doesn't expose the `GetTypeByName` method. [LK:2019-06-06]
                    var type = TypeFinder.GetTypeByName(item.Value<string>("type"));

                    if (type != null)
                    {
                        var serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
                        {
                            ContractResolver = new ConfigurationFieldContractResolver(),
                            Converters = new List<JsonConverter>(new[] { new FuzzyBooleanConverter() })
                        });

                        if (item["value"].ToObject(type, serializer) is IConfigurationEditorItem obj)
                        {
                            items.Add(obj);
                        }
                    }
                }

                return items;
            }

            return base.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
        }
    }
}
