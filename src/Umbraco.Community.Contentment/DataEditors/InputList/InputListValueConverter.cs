// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

using System.Collections;
using System.Collections.Concurrent;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors;

internal sealed class InputListValueConverter : PropertyValueConverterBase, IDeliveryApiPropertyValueConverter
{
    private static readonly ConcurrentDictionary<Guid, IPublishedPropertyType> _propertyTypeLookup = new();

    // NOTE: Needs to use `int` ID, as `propertyType.DataType` doesn't have the Guid key. [LK]
    private static readonly ConcurrentDictionary<int, Type> _tupleTypeLookup = new();

    private const int _defaultDataTypeId = UmbConstants.DataTypes.LabelString;

    private readonly IIdKeyMap _idKeyMap;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IPublishedContentTypeFactory _publishedContentTypeFactory;


    public InputListValueConverter(
        IIdKeyMap idKeyMap,
        IJsonSerializer jsonSerializer,
        IPublishedContentTypeFactory publishedContentTypeFactory)
    {
        _idKeyMap = idKeyMap;
        _jsonSerializer = jsonSerializer;
        _publishedContentTypeFactory = publishedContentTypeFactory;
    }

    public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(InputListDataEditor.DataEditorAlias);

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => GetPropertyValueTypeImpl(propertyType, isDeliveryApi: false);

    public override object? ConvertSourceToIntermediate(
        IPublishedElement owner,
        IPublishedPropertyType propertyType,
        object? source,
        bool preview)
    {
        if (source is string value)
        {
            if (value.DetectIsJson() == false)
            {
                return value;
            }

            var model = _jsonSerializer.Deserialize<InputListValueModel>(value);
            if (model is not null)
            {
                foreach (var row in model)
                {
                    foreach (var item in row.Values)
                    {
                        var propType = GetPublishedPropertyType(propertyType, item.DataType);
                        if (propType is not null)
                        {
                            item.Value = propType.ConvertSourceToInter(owner, item.Value, preview);
                        }
                    }
                }

                return model;
            }
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

    // NOTE: The internal cache is cleared from `ContentmentDataTypeNotificationHandler` [LK]
    internal static void ClearCache(IDataType dataType)
    {
        _propertyTypeLookup.TryRemove(dataType.Key, out _);
        _tupleTypeLookup.TryRemove(dataType.Id, out _);
    }

    private Type GetPropertyValueTypeImpl(IPublishedPropertyType propertyType, bool isDeliveryApi)
    {
        if (_tupleTypeLookup.TryGetValue(propertyType.DataType.Id, out var tupleType) == false)
        {
            var clrTypes = new List<Type>();

            if (propertyType.DataType.ConfigurationObject is Dictionary<string, object> configuration &&
               configuration.TryGetValue("columns", out var tmp) == true &&
               tmp is not null)
            {
                // Config is stored as Array<{ key, dataType, value: Record<string,unknown> }>.
                // Re-serialize to JSON and deserialize to a known type for safe extraction of the DataType guid.
                var json = _jsonSerializer.Serialize(tmp);
                var columns = _jsonSerializer.Deserialize<List<InputListColumnConfigDto>>(json);
                if (columns?.Count > 0)
                {
                    foreach (var column in columns)
                    {
                        if (Guid.TryParse(column.DataType, out var dataTypeKey) == true)
                        {
                            var clrType = GetClrType(propertyType, dataTypeKey, isDeliveryApi);
                            clrTypes.Add(clrType);
                        }
                        else
                        {
                            clrTypes.Add(typeof(object));
                        }
                    }
                }
            }

            if (clrTypes.Count == 0)
            {
                // No columns configured — store a sentinel and return a plain IEnumerable<object>.
                // ConvertIntermediateToObjectImpl will detect typeof(object) and fall through.
                tupleType = typeof(object);
            }
            else
            {
                // C# concrete type for `Tuple<>` has a maximum of 8 generic parameters.
                // For >7 columns we nest recursively: Tuple<T1..T7, Tuple<T8..>>.
                // ref: https://learn.microsoft.com/en-us/dotnet/api/system.tuple-8
                tupleType = BuildTupleType(clrTypes);
            }

            _tupleTypeLookup.TryAdd(propertyType.DataType.Id, tupleType);
        }

        return typeof(IEnumerable<>).MakeGenericType(tupleType);
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
        if (inter is InputListValueModel model &&
            propertyType.ClrType is not null &&
            _tupleTypeLookup.TryGetValue(propertyType.DataType.Id, out var tupleType) == true &&
            tupleType != typeof(object)) // typeof(object) is the sentinel for a zero-column configuration
        {
            var array = Array.CreateInstance(tupleType, model.Count);

            for (var i = 0; i < model.Count; i++)
            {
                var items = model[i].Values;
                var args = new object?[items.Count];

                for (var j = 0; j < items.Count; j++)
                {
                    var item = items[j];

                    if (_propertyTypeLookup.TryGetValue(item.DataType, out var propType) == true)
                    {
                        var value = isDeliveryApi
                            ? propType.ConvertInterToDeliveryApiObject(owner, referenceCacheLevel, item.Value, preview, expanding)
                            : propType.ConvertInterToObject(owner, referenceCacheLevel, item.Value, preview);

                        args[j] = value;
                    }
                }

                var tuple = CreateTupleInstance(tupleType, args);
                array.SetValue(tuple, i);
            }

            return array;
        }

        return base.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
    }

    private Type GetClrType(IPublishedPropertyType owner, Guid dataTypeKey, bool isDeliveryApi)
    {
        var propertyType = GetPublishedPropertyType(owner, dataTypeKey);

        return propertyType is not null
            ? (isDeliveryApi == true ? propertyType.DeliveryApiModelClrType : propertyType.ModelClrType) ?? typeof(object)
            : typeof(object);
    }

    private IPublishedPropertyType? GetPublishedPropertyType(IPublishedPropertyType owner, Guid dataTypeKey)
    {
        if (_propertyTypeLookup.TryGetValue(dataTypeKey, out var propertyType) == true && propertyType is not null)
        {
            return propertyType;
        }

        if (owner.ContentType is not null)
        {
            var dataTypeId = _idKeyMap.GetIdForKey(dataTypeKey, UmbracoObjectTypes.DataType).ResultOr(_defaultDataTypeId);

            propertyType = _publishedContentTypeFactory.CreatePropertyType(
            owner.ContentType,
            owner.Alias,
            dataTypeId,
            ContentVariation.Nothing);

            if (propertyType is not null)
            {
                _ = _propertyTypeLookup.TryAdd(dataTypeKey, propertyType);
            }
        }

        return propertyType;
    }

    /// <summary>
    /// Recursively builds a nested <see cref="System.Tuple{TRest}"/> type for any number of generic args.
    /// Groups of 7 are wrapped with a TRest tail tuple to bypass the 8-parameter limit.
    /// </summary>
    private static Type BuildTupleType(IList<Type> types, int offset = 0)
    {
        var count = types.Count - offset;
        if (count <= 7)
        {
            var slice = types.Skip(offset).Take(count).ToArray();
            var tupleOpen = Type.GetType($"System.Tuple`{count}")!;
            return tupleOpen.MakeGenericType(slice);
        }

        // count >= 8: take first 7, recurse for rest
        var firstSeven = types.Skip(offset).Take(7).ToArray();
        var restType = BuildTupleType(types, offset + 7);
        var tuple8 = Type.GetType("System.Tuple`8")!;
        return tuple8.MakeGenericType([.. firstSeven, restType]);
    }

    /// <summary>
    /// Recursively creates a nested Tuple instance from a flat array of values, matching the shape of
    /// <paramref name="tupleType"/> produced by <see cref="BuildTupleType"/>.
    /// </summary>
    private static object? CreateTupleInstance(Type tupleType, object?[] allArgs, int offset = 0)
    {
        var typeArgs = tupleType.GetGenericArguments();

        if (typeArgs.Length <= 7)
        {
            var args = new object?[typeArgs.Length];
            for (var k = 0; k < typeArgs.Length; k++)
            {
                args[k] = (offset + k < allArgs.Length) ? allArgs[offset + k] : null;
            }
            return Activator.CreateInstance(tupleType, args);
        }

        // 8 args: last is TRest
        var outerArgs = new object?[8];
        for (var k = 0; k < 7; k++)
        {
            outerArgs[k] = (offset + k < allArgs.Length) ? allArgs[offset + k] : null;
        }
        outerArgs[7] = CreateTupleInstance(typeArgs[7], allArgs, offset + 7);
        return Activator.CreateInstance(tupleType, outerArgs);
    }

    // Config is stored as Array<{ key: string; dataType: string; value: Record<string, unknown> }>.
    // The `value` bag (label, future fields) is config-display only — the converter only needs key + dataType.
    private sealed class InputListColumnConfigDto
    {
        public string Key { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
    }
}
