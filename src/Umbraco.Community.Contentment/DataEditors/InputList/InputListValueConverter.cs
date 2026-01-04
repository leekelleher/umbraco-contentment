// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

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
    private static readonly Dictionary<Guid, IPublishedPropertyType> _propertyTypeLookup = [];

    // NOTE: Needs to use `int` ID, as `propertyType.DataType` doesn't have the Guid key. [LK]
    private static readonly Dictionary<int, Type> _tupleTypeLookup = [];

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

            var model = _jsonSerializer.Deserialize<List<List<InputListValueModel>>>(value);
            if (model is not null)
            {
                foreach (var row in model)
                {
                    foreach (var item in row)
                    {
                        var propType = GetPublishedPropertyType(propertyType, item.Alias);
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
        _ = _propertyTypeLookup.Remove(dataType.Key);
        _ = _tupleTypeLookup.Remove(dataType.Id);
    }

    private Type GetPropertyValueTypeImpl(IPublishedPropertyType propertyType, bool isDeliveryApi)
    {
        if (_tupleTypeLookup.TryGetValue(propertyType.DataType.Id, out var tupleType) == false)
        {
            var clrTypes = new List<Type>();

            if (propertyType.DataType.ConfigurationObject is Dictionary<string, object> configuration &&
               configuration.TryGetValue("dataTypes", out var tmp) == true &&
               tmp is List<string> items)
            {
                foreach (var item in items)
                {
                    if (Guid.TryParse(item, out var dataTypeKey) == true)
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

            // C# concrete type for `Tuple<>` has a maximum of 8 generic parameters.
            // ref: https://learn.microsoft.com/en-us/dotnet/api/system.tuple-8
            var tuple = Type.GetType($"System.Tuple`{Math.Min(clrTypes.Count, 8)}");
            tupleType = tuple!.MakeGenericType(clrTypes.ToArray());

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
        if (inter is List<List<InputListValueModel>> model &&
            _tupleTypeLookup.TryGetValue(propertyType.DataType.Id, out var tupleType) == true)
        {
            var array = Array.CreateInstance(tupleType, model.Count);

            for (var i = 0; i < model.Count; i++)
            {
                var items = model[i];
                var args = new object?[items.Count];

                for (var j = 0; j < items.Count; j++)
                {
                    var item = items[j];

                    if (_propertyTypeLookup.TryGetValue(item.Alias, out var propType) == true)
                    {
                        var value = isDeliveryApi
                            ? propType.ConvertInterToDeliveryApiObject(owner, referenceCacheLevel, item.Value, preview, expanding)
                            : propType.ConvertInterToObject(owner, referenceCacheLevel, item.Value, preview);

                        args[j] = value;
                    }
                }

                var tuple = Activator.CreateInstance(tupleType, args);
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
}
