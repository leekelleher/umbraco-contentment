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
            var defaultValueType = typeof(string);

            if (propertyType.DataType.Configuration is Dictionary<string, object> configuration &&
                configuration.TryGetValue(DataListConfigurationEditor.DataSource, out var tmp1) &&
                tmp1 is JArray array1 && array1.Count > 0 && array1[0] is JObject obj1 &&
                configuration.TryGetValue(DataListConfigurationEditor.ListEditor, out var tmp2) &&
                tmp2 is JArray array2 && array2.Count > 0 && array2[0] is JObject obj2)
            {
                var valueType = default(Type);

                // NOTE: Patches a breaking-change. I'd renamed `type` to become `key`. [LK:2020-04-03]
                if (obj1.ContainsKey("key") == false && obj1.ContainsKey("type"))
                {
                    obj1.Add("key", obj1["type"]);
                    obj1.Remove("type");
                }

                var source = _utility.GetConfigurationEditor<IDataListSourceValueConverter>(obj1.Value<string>("key"));
                if (source != null)
                {
                    var config = obj1["value"].ToObject<Dictionary<string, object>>();
                    valueType = source.GetValueType(config);
                }

                // NOTE: Patches a breaking-change. I'd renamed `type` to become `key`. [LK:2020-04-03]
                if (obj2.ContainsKey("key") == false && obj2.ContainsKey("type"))
                {
                    obj2.Add("key", obj2["type"]);
                    obj2.Remove("type");
                }

                var editor = _utility.GetConfigurationEditor<IDataListEditor>(obj2.Value<string>("key"));

                return editor?.HasMultipleValues == true
                    ? typeof(IEnumerable<>).MakeGenericType(valueType ?? defaultValueType)
                    : valueType ?? defaultValueType;
            }

            return defaultValueType;
        }

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            if (source is string value)
            {
                if (value.DetectIsJson() == false)
                {
                    return value;
                }

                return JsonConvert.DeserializeObject<IEnumerable<string>>(value);
            }

            return base.ConvertSourceToIntermediate(owner, propertyType, source, preview);
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            var converter = default(Func<Type, string, object>);
            var valueType = typeof(string);

            if (propertyType.DataType.Configuration is Dictionary<string, object> configuration &&
                configuration.TryGetValue(DataListConfigurationEditor.DataSource, out var tmp1) &&
                tmp1 is JArray array1 && array1.Count > 0 && array1[0] is JObject item1)
            {
                // NOTE: Patches a breaking-change. I'd renamed `type` to become `key`. [LK:2020-04-03]
                if (item1.ContainsKey("key") == false && item1.ContainsKey("type"))
                {
                    item1.Add("key", item1["type"]);
                    item1.Remove("type");
                }

                var source = _utility.GetConfigurationEditor<IDataListSourceValueConverter>(item1.Value<string>("key"));
                if (source != null)
                {
                    var config = item1["value"].ToObject<Dictionary<string, object>>();
                    valueType = source.GetValueType(config);
                    converter = source.ConvertValue;
                }
            }

            if (inter is string value)
            {
                return converter != null
                    ? converter(valueType, value)
                    : value;
            }

            if (inter is IEnumerable<string> items)
            {
                if (propertyType.ClrType.IsGenericType)
                {
                    var objects = new List<object>();

                    foreach (var item in items)
                    {
                        objects.Add(converter != null ? converter(valueType, item) : item);
                    }

                    var result = Array.CreateInstance(valueType, objects.Count);
                    for (var i = 0; i < objects.Count; i++)
                    {
                        var attempt = objects[i].TryConvertTo(valueType);
                        if (attempt.Success)
                        {
                            result.SetValue(attempt.Result, i);
                        }
                        else
                        {
                            // NOTE: At this point `TryConvertTo` can't convert to the `valueType`.
                            // This may be a case where the `valueType` is an interface.
                            // We can attempt to cast it directly, as a last resort.
                            if (valueType.IsInstanceOfType(objects[i]))
                            {
                                result.SetValue(objects[i], i);
                            }
                        }
                    }

                    return result;
                }

                return string.Join(",", items);
            }

            return base.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
        }
    }
}
