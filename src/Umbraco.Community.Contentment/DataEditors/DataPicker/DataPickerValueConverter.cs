/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections;
using System.Text.Json;
using System.Text.Json.Nodes;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class DataPickerValueConverter : PropertyValueConverterBase, IDeliveryApiPropertyValueConverter
    {
        private readonly Type _defaultObjectType = typeof(string);
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ConfigurationEditorUtility _utility;

        public DataPickerValueConverter(
            IJsonSerializer jsonSerializer,
            ConfigurationEditorUtility utility)
        {
            _jsonSerializer = jsonSerializer;
            _utility = utility;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(DataPickerDataEditor.DataEditorAlias);

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => GetPropertyValueTypeImpl(propertyType, isDeliveryApi: false);

        public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview)
        {
            if (source is string value)
            {
                if (value.DetectIsJson() == false)
                {
                    return value;
                }

                return _jsonSerializer.Deserialize<IEnumerable<string>>(value);
            }

            return base.ConvertSourceToIntermediate(owner, propertyType, source, preview);
        }

        public override object? ConvertIntermediateToObject(
            IPublishedElement owner,
            IPublishedPropertyType propertyType,
            PropertyCacheLevel referenceCacheLevel,
            object? inter,
            bool preview) => ConvertIntermediateToObjectImpl(owner, propertyType, referenceCacheLevel, inter, preview, isDeliveryApi: false, expanding: false);

        public PropertyCacheLevel GetDeliveryApiPropertyCacheLevel(IPublishedPropertyType propertyType) => GetPropertyCacheLevel(propertyType);

        public Type GetDeliveryApiPropertyValueType(IPublishedPropertyType propertyType) => GetPropertyValueTypeImpl(propertyType, isDeliveryApi: true);

        public object? ConvertIntermediateToDeliveryApiObject(
            IPublishedElement owner,
            IPublishedPropertyType propertyType,
            PropertyCacheLevel referenceCacheLevel,
            object? inter,
            bool preview,
            bool expanding) => ConvertIntermediateToObjectImpl(owner, propertyType, referenceCacheLevel, inter, preview, isDeliveryApi: true, expanding);

        private void TryGetPropertyTypeConfiguration(
            IPublishedPropertyType propertyType,
            out bool hasMultipleValues,
            out Type valueType,
            out Func<Type, string, object?>? converter,
            bool isDeliveryApi = false,
            bool expanding = false)
        {
            hasMultipleValues = false;
            valueType = _defaultObjectType;
            converter = default;

            if (propertyType.DataType.ConfigurationObject is Dictionary<string, object> configuration &&
                configuration.TryGetValue("dataSource", out var tmp1) == true &&
                tmp1 is JsonArray array1 && array1.Count > 0 && array1[0] is JsonObject obj1 &&
                obj1.GetValueAsString("key") is string key1)
            {
                var source = _utility.GetConfigurationEditor<IDataSourceValueConverter>(key1);
                if (source is not null)
                {
                    var config = obj1["value"]?.Deserialize<Dictionary<string, object>>();

                    if (isDeliveryApi && source is IDataSourceDeliveryApiValueConverter deliveryApiSource)
                    {
                        valueType = deliveryApiSource.GetDeliveryApiValueType(config) ?? _defaultObjectType;
                        converter = (t, v) => deliveryApiSource.ConvertToDeliveryApiValue(t, v, expanding);
                    }
                    else
                    {
                        valueType = source.GetValueType(config) ?? _defaultObjectType;
                        converter = source.ConvertValue;
                    }
                }

                hasMultipleValues = configuration.TryGetValue("maxItems", out var tmp2) == true && tmp2.TryConvertTo<int>().ResultOr(0) != 1;
            }
        }

        private Type GetPropertyValueTypeImpl(IPublishedPropertyType propertyType, bool isDeliveryApi)
        {
            TryGetPropertyTypeConfiguration(propertyType, out var hasMultipleValues, out var valueType, out _, isDeliveryApi);

            return hasMultipleValues == true
                ? typeof(List<>).MakeGenericType(valueType)
                : valueType;
        }

        private object? ConvertIntermediateToObjectImpl(
            IPublishedElement owner,
            IPublishedPropertyType propertyType,
            PropertyCacheLevel referenceCacheLevel,
            object? inter,
            bool preview,
            bool isDeliveryApi,
            bool expanding)
        {
            TryGetPropertyTypeConfiguration(propertyType, out var hasMultipleValues, out var valueType, out var converter, isDeliveryApi, expanding);

            if (inter is string value)
            {
                // EDGE-CASE: Values previously saved as single-value, but switched to multi-value, without re-saving it'd be null.
                // ref: https://github.com/leekelleher/umbraco-contentment/issues/226#issue-1266583794
                if (hasMultipleValues == true)
                {
                    inter = value.AsEnumerableOfOne();
                }
                else
                {
                    return converter != null
                        ? converter(valueType, value)
                        : value;
                }
            }

            // EDGE-CASE: To work around Umbraco `PublishedElementPropertyBase` not calling `ConvertSourceToIntermediate()` [LK:2021-05-25]
            // ref: https://github.com/leekelleher/umbraco-contentment/issues/111#issuecomment-847780287
            if (inter is JsonArray array)
            {
                inter = array.Deserialize<IEnumerable<string>>() ?? Enumerable.Empty<string>();
            }

            if (inter is IEnumerable<string> items)
            {
                if (hasMultipleValues == true)
                {
                    var result = Activator.CreateInstance(typeof(List<>).MakeGenericType(valueType)) as IList;

                    foreach (var item in items)
                    {
                        var obj = converter != null
                            ? converter(valueType, item)
                            : item;

                        if (obj != null)
                        {
                            var attempt = obj.TryConvertTo(valueType);
                            if (attempt.Success == true)
                            {
                                _ = result?.Add(attempt.Result);
                            }
                            else
                            {
                                // NOTE: At this point `TryConvertTo` can't convert to the `valueType`.
                                // This may be a case where the `valueType` is an interface.
                                // We can attempt to cast it directly, as a last resort.
                                if (valueType.IsInstanceOfType(obj) == true)
                                {
                                    _ = result?.Add(obj);
                                }
                            }
                        }
                    }

                    return result;
                }
                else
                {
                    // NOTE: When the `inter` is enumerable, but `hasMultipleValues` is false, take the first item value.
                    foreach (var item in items)
                    {
                        return converter != null
                            ? converter(valueType, item)
                            : item;
                    }
                }

                // NOTE: This is the last resort. Comma-separated string.
                return string.Join(",", items);
            }

            return base.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
        }
    }
}
