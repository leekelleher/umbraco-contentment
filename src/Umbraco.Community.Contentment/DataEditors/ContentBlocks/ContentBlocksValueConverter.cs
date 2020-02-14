/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Umbraco.Community.Contentment.Web.PublishedCache;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.PublishedCache;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Core.Composing.HideFromTypeFinder]
    public sealed class ContentBlocksValueConverter : PropertyValueConverterBase
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IPublishedModelFactory _publishedModelFactory;
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

        public ContentBlocksValueConverter(
            IContentTypeService contentTypeService,
            IPublishedModelFactory publishedModelFactory,
            IPublishedSnapshotAccessor publishedSnapshotAccessor)
            : base()
        {
            _contentTypeService = contentTypeService;
            _publishedModelFactory = publishedModelFactory;
            _publishedSnapshotAccessor = publishedSnapshotAccessor;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(ContentBlocksDataEditor.DataEditorAlias);

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(IEnumerable<IPublishedElement>);

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            if (source is string value)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ContentBlock>>(value);
            }

            return base.ConvertSourceToIntermediate(owner, propertyType, source, preview);
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if (inter is IEnumerable<ContentBlock> items)
            {
                var elements = new List<IPublishedElement>();

                foreach (var item in items)
                {
                    if (item == null || item.ElementType.Equals(Guid.Empty))
                        continue;

                    // NOTE: [LK:2019-09-03] Why `IPublishedCache` doesn't support Guids or UDIs, I do not know!?
                    // Thought v8 was meant to be "GUID ALL THE THINGS!!1"? ¯\_(ツ)_/¯
                    if (ContentTypeCacheHelper.TryGetAlias(item.ElementType, out var alias, _contentTypeService) == false)
                        continue;

                    var contentType = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetContentType(alias);
                    if (contentType == null || contentType.IsElement == false)
                        continue;

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
    }
}
