/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models.DeliveryApi;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Community.Contentment.Web.PublishedCache;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class ContentBlocksValueConverter : PropertyValueConverterBase, IDeliveryApiPropertyValueConverter
    {
        private readonly IApiElementBuilder _apiElementBuilder;
        private readonly IPublishedModelFactory _publishedModelFactory;
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

        public ContentBlocksValueConverter(
            IApiElementBuilder apiElementBuilder,
            IPublishedModelFactory publishedModelFactory,
            IPublishedSnapshotAccessor publishedSnapshotAccessor)
            : base()
        {
            _apiElementBuilder = apiElementBuilder;
            _publishedModelFactory = publishedModelFactory;
            _publishedSnapshotAccessor = publishedSnapshotAccessor;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(ContentBlocksDataEditor.DataEditorAlias);

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(IEnumerable<IPublishedElement>);

        public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview)
        {
            if (source is string value)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ContentBlock>>(value);
            }

            return base.ConvertSourceToIntermediate(owner, propertyType, source, preview);
        }

        public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
        {
            if (inter is IEnumerable<ContentBlock> items)
            {
                var elements = new List<IPublishedElement>();

                foreach (var item in items)
                {
                    if (item == null || item.ElementType == Guid.Empty)
                    {
                        continue;
                    }

                    var contentType = _publishedSnapshotAccessor
                        .GetRequiredPublishedSnapshot()
                        .Content?
                        .GetContentType(item.ElementType);

                    if (contentType == null || contentType.IsElement == false)
                    {
                        continue;
                    }

                    var properties = new List<IPublishedProperty>();

                    foreach (var thing in item.Value)
                    {
                        var propType = contentType.GetPropertyType(thing.Key);
                        if (propType != null)
                        {
                            properties.Add(new DetachedPublishedProperty(propType, owner, thing.Value, preview));
                        }
                    }

                    elements.Add(_publishedModelFactory.CreateModel(new DetachedPublishedElement(item.Key, contentType, properties)));
                }

                return elements;
            }

            return base.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
        }

        public PropertyCacheLevel GetDeliveryApiPropertyCacheLevel(IPublishedPropertyType propertyType) => GetPropertyCacheLevel(propertyType);

        public Type GetDeliveryApiPropertyValueType(IPublishedPropertyType propertyType) => typeof(IEnumerable<IApiElement>);

        public object? ConvertIntermediateToDeliveryApiObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview, bool expanding)
        {
            var items = ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview) as IEnumerable<IPublishedElement>;

            return items?.Any() == true
                ? items.Select(_apiElementBuilder.Build).ToArray()
                : Array.Empty<IApiElement>();
        }
    }
}
