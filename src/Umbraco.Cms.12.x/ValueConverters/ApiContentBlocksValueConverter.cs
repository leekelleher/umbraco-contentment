using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models.DeliveryApi;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Community.Contentment.DataEditors;

namespace Umbraco.Cms.v12_x.ValueConverters;

public class ApiContentBlocksValueConverter : PropertyValueConverterBase, IDeliveryApiPropertyValueConverter
{
    private readonly ContentBlocksValueConverter _contentBlocksValueConverter;
    private readonly IApiElementBuilder _apiElementBuilder;

    public ApiContentBlocksValueConverter(
        IApiElementBuilder apiElementBuilder,
        IContentTypeService contentTypeService,
        IPublishedModelFactory publishedModelFactory,
        IPublishedSnapshotAccessor publishedSnapshotAccessor)
        : base()
    {
        _apiElementBuilder = apiElementBuilder;

        _contentBlocksValueConverter = new ContentBlocksValueConverter(
            contentTypeService,
            publishedModelFactory,
            publishedSnapshotAccessor);
    }

    public override bool IsConverter(IPublishedPropertyType propertyType)
        => _contentBlocksValueConverter.IsConverter(propertyType);

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        => _contentBlocksValueConverter.GetPropertyValueType(propertyType);

    public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview)
        => _contentBlocksValueConverter.ConvertSourceToIntermediate(owner, propertyType, source, preview);

    public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
        => _contentBlocksValueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);

    public PropertyCacheLevel GetDeliveryApiPropertyCacheLevel(IPublishedPropertyType propertyType)
        => GetPropertyCacheLevel(propertyType);

    public Type GetDeliveryApiPropertyValueType(IPublishedPropertyType propertyType)
        => typeof(IEnumerable<IApiElement>);

    public object? ConvertIntermediateToDeliveryApiObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview, bool expanding)
    {
        var items = ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview) as IEnumerable<IPublishedElement>;

        return items?.Any() == true
            ? items.Select(_apiElementBuilder.Build).ToArray()
            : Array.Empty<IApiElement>();
    }
}
