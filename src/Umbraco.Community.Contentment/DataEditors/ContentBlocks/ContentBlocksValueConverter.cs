/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Umbraco.Community.Contentment.Web.PublishedCache;
#if NET472
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PublishedCache;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class ContentBlocksValueConverter : PropertyValueConverterBase
    {
        private readonly IPublishedModelFactory _publishedModelFactory;
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

        public ContentBlocksValueConverter(
            IPublishedModelFactory publishedModelFactory,
            IPublishedSnapshotAccessor publishedSnapshotAccessor)
            : base()
        {
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
                var contentCache = _publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content;
                var elements = new List<IPublishedElement>();

                foreach (var item in items)
                {
                    if (item == null || item.ElementType == Guid.Empty)
                        continue;

                    var contentType = contentCache.GetContentType(item.ElementType);
                    if (contentType == null)
                        continue;

                    switch (item.Udi?.EntityType)
                    {
                        case UmbConstants.UdiEntityType.Document:
                            {
                                var content = contentCache.GetById(item.Key);
                                if (content != null)
                                {
                                    elements.Add(_publishedModelFactory.CreateModel(content));
                                }
                            }
                            break;

                        case UmbConstants.UdiEntityType.Element:
                        default:
                            {
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
                            break;
                    }
                }

                return elements;
            }

            return base.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
        }
    }
}
