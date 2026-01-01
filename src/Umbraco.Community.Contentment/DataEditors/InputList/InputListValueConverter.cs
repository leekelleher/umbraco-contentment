// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors;

internal sealed class InputListValueConverter : PropertyValueConverterBase, IDeliveryApiPropertyValueConverter
{
    private readonly IDataTypeService _dataTypeService;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IPublishedContentTypeFactory _publishedContentTypeFactory;
    private readonly IShortStringHelper _shortStringHelper;

    public InputListValueConverter(
        IDataTypeService dataTypeService,
        IJsonSerializer jsonSerializer,
        IPublishedContentTypeFactory publishedContentTypeFactory,
        IShortStringHelper shortStringHelper)
    {
        _dataTypeService = dataTypeService;
        _jsonSerializer = jsonSerializer;
        _publishedContentTypeFactory = publishedContentTypeFactory;
        _shortStringHelper = shortStringHelper;
    }

    public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(InputListDataEditor.DataEditorAlias);

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(IEnumerable<IEnumerable<object?>?>);

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

            return _jsonSerializer.Deserialize<List<List<InputListValueModel>>>(value);
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

    public Type GetDeliveryApiPropertyValueType(IPublishedPropertyType propertyType) => GetPropertyValueType(propertyType);

    public object? ConvertIntermediateToDeliveryApiObject(
         IPublishedElement owner,
         IPublishedPropertyType propertyType,
         PropertyCacheLevel referenceCacheLevel,
         object? inter,
         bool preview,
         bool expanding) => ConvertIntermediateToObjectImpl(owner, propertyType, referenceCacheLevel, inter, preview, isDeliveryApi: true, expanding);

    private object? ConvertIntermediateToObjectImpl(
            IPublishedElement owner,
            IPublishedPropertyType propertyType,
            PropertyCacheLevel referenceCacheLevel,
            object? inter,
            bool preview,
            bool isDeliveryApi,
            bool expanding)
    {
        if (inter is List<List<InputListValueModel>> items && items.Count > 0)
        {
            var outerList = new List<List<object?>?>();

            if (propertyType.ContentType is not null)
            {
                var dataTypeKeys = items[0]
                    .Select(x => x.Alias)
                    .Where(x => Guid.Empty.Equals(x) == false)
                    .ToArray();

                var dataTypes = _dataTypeService.GetAllAsync(dataTypeKeys).GetAwaiter().GetResult();
                var lookup = dataTypes.ToDictionary(x => x.Key);

                foreach (var item in items)
                {
                    var innerList = new List<object?>();

                    foreach (var property in item)
                    {
                        if (lookup.TryGetValue(property.Alias, out var dataType) == true)
                        {
                            var publishedPropertyType = _publishedContentTypeFactory.CreatePropertyType(
                                propertyType.ContentType, new PropertyType(_shortStringHelper, dataType, propertyType.Alias));

                            var inter1 = publishedPropertyType.ConvertSourceToInter(owner, property.Value, preview);

                            var obj1 = isDeliveryApi
                                ? publishedPropertyType.ConvertInterToDeliveryApiObject(owner, referenceCacheLevel, inter1, preview, expanding)
                                : publishedPropertyType.ConvertInterToObject(owner, referenceCacheLevel, inter1, preview);

                            innerList.Add(obj1);
                        }
                    }

                    outerList.Add(innerList);
                }
            }

            return outerList;
        }

        return base.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
    }
}
