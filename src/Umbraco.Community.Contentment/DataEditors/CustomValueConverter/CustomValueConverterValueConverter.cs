/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
#if NET472
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
#else
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class CustomValueConverterValueConverter : PropertyValueConverterBase
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly PropertyValueConverterCollection _propertyValueConverters;
        private readonly IPublishedContentTypeFactory _publishedContentTypeFactory;

        public CustomValueConverterValueConverter(
            IDataTypeService dataTypeService,
            PropertyValueConverterCollection propertyValueConverters,
            IPublishedContentTypeFactory publishedContentTypeFactory)
        {
            _dataTypeService = dataTypeService;
            _propertyValueConverters = propertyValueConverters;
            _publishedContentTypeFactory = publishedContentTypeFactory;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(CustomValueConverterDataEditor.DataEditorAlias);

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            return TryGetPropertyValueConverter(propertyType, out var converter) == true
                ? converter.GetPropertyValueType(propertyType)
                : typeof(object);
        }

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            return TryGetPropertyValueConverter(propertyType, out var converter) == true
                ? converter.ConvertSourceToIntermediate(owner, propertyType, source, preview)
                : base.ConvertSourceToIntermediate(owner, propertyType, source, preview);
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            return TryGetPropertyValueConverter(propertyType, out var converter) == true
                ? converter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview)
                : base.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
        }

        public override object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            return TryGetPropertyValueConverter(propertyType, out var converter) == true
                ? converter.ConvertIntermediateToXPath(owner, propertyType, referenceCacheLevel, inter, preview)
                : base.ConvertIntermediateToXPath(owner, propertyType, referenceCacheLevel, inter, preview);
        }

        private bool TryGetPropertyValueConverter(IPublishedPropertyType propertyType, out IPropertyValueConverter converter)
        {
            // TODO: [LK] Look at caching this, as it is called every time! ಠ_ಠ

            if (propertyType.DataType.Configuration is Dictionary<string, object> configuration)
            {
                // NOTE: Try to get the custom value converter first, otherwise fallback on the property's own converter.
                if (configuration.TryGetValueAs("valueConverter", out JArray array) == true &&
                    array.Count > 0 &&
                    array[0].Value<string>() is string valueConverter &&
                    string.IsNullOrWhiteSpace(valueConverter) == false)
                {
                    converter = _propertyValueConverters.FirstOrDefault(x => valueConverter.InvariantEquals(x.GetType().FullName) == true);

                    if (converter != default)
                    {
                        return true;
                    }
                }

                if (configuration.TryGetValueAs(CustomValueConverterConfigurationEditor.DataType, out GuidUdi udi) == true)
                {
                    var dataType = _dataTypeService.GetDataType(udi.Guid);
                    if (dataType != null)
                    {
                        var innerPropertyType = _publishedContentTypeFactory.CreatePropertyType(
                            propertyType.ContentType,
                            propertyType.Alias,
                            dataType.Id,
                            ContentVariation.Nothing);

                        converter = _propertyValueConverters.FirstOrDefault(x => x.IsConverter(innerPropertyType) == true);

                        if (converter != default)
                        {
                            return true;
                        }
                    }
                }
            }

            converter = default;
            return false;
        }
    }

    // TODO: [LK] Remove this class. It is only here as an initial example.
    public class ReverseMyStringValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(IPublishedPropertyType propertyType) => false;

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(string);

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if (inter is string str)
            {
                return new string(str.Reverse().ToArray());
            }

            return Guid.NewGuid().ToString();
        }
    }
}
