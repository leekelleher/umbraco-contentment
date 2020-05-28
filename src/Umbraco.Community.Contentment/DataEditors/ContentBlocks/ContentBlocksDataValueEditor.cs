/* Copyright © 2014 Umbrella Inc, Our Umbraco and other contributors.
 * This Source Code has been derived from Nested Content.
 * https://github.com/umco/umbraco-nested-content/blob/0.5.0/src/Our.Umbraco.NestedContent/PropertyEditors/NestedContentPropertyEditor.cs
 * Including derivations made in Umbraco CMS for v8. Copyright © 2013-present Umbraco.
 * https://github.com/umbraco/Umbraco-CMS/blob/release-8.4.0/src/Umbraco.Web/PropertyEditors/NestedContentPropertyEditor.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ContentBlocksDataValueEditor : DataValueEditor
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly Lazy<Dictionary<Guid, IContentType>> _elementTypes;
        private readonly PropertyEditorCollection _propertyEditors;

        public ContentBlocksDataValueEditor(
            IContentTypeService contentTypeService,
            IDataTypeService dataTypeService,
            PropertyEditorCollection propertyEditors)
            : base()
        {
            _dataTypeService = dataTypeService;
            _elementTypes = new Lazy<Dictionary<Guid, IContentType>>(() => contentTypeService.GetAllElementTypes().ToDictionary(x => x.Key));
            _propertyEditors = propertyEditors;
        }

        public override object ToEditor(Property property, IDataTypeService dataTypeService, string culture = null, string segment = null)
        {
            var value = property.GetValue(culture, segment)?.ToString();
            if (string.IsNullOrWhiteSpace(value))
            {
                return base.ToEditor(property, dataTypeService, culture, segment);
            }

            // TODO: [LK] Could check if the value has come from NestedContent or StackedContent?
            // ncContentTypeAlias, icContentTypeAlias
            // Could use a custom JsonConverter? a la: https://stackoverflow.com/a/43714309/12787
            // or https://www.jerriepelser.com/blog/deserialize-different-json-object-same-class/

            var blocks = JsonConvert.DeserializeObject<IEnumerable<ContentBlock>>(value);
            if (blocks == null)
            {
                return base.ToEditor(property, dataTypeService, culture, segment);
            }

            foreach (var block in blocks)
            {
                var elementType = _elementTypes.Value.ContainsKey(block.ElementType)
                    ? _elementTypes.Value[block.ElementType]
                    : null;

                if (elementType == null)
                {
                    continue;
                }

                var keys = new List<string>(block.Value.Keys);
                foreach (var key in keys)
                {
                    var propertyType = elementType.CompositionPropertyTypes.FirstOrDefault(x => x.Alias.InvariantEquals(key));
                    if (propertyType == null)
                    {
                        continue;
                    }

                    propertyType.Variations = ContentVariation.Nothing;

                    var fakeProperty = new Property(propertyType);
                    fakeProperty.SetValue(block.Value[key]);

                    var propertyEditor = _propertyEditors[propertyType.PropertyEditorAlias];
                    if (propertyEditor == null)
                    {
                        block.Value[key] = fakeProperty.GetValue()?.ToString();
                        continue;
                    }

                    var convertedValue = propertyEditor.GetValueEditor()?.ToEditor(fakeProperty, dataTypeService);

                    block.Value[key] = convertedValue != null
                        ? JToken.FromObject(convertedValue)
                        : null;
                }
            }

            return blocks;
        }

        public override object FromEditor(ContentPropertyData editorValue, object currentValue)
        {
            var value = editorValue?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(value))
            {
                return base.FromEditor(editorValue, currentValue);
            }

            var blocks = JsonConvert.DeserializeObject<IEnumerable<ContentBlock>>(value);
            if (blocks == null)
            {
                return base.FromEditor(editorValue, currentValue);
            }

            foreach (var block in blocks)
            {
                var elementType = _elementTypes.Value.ContainsKey(block.ElementType)
                    ? _elementTypes.Value[block.ElementType]
                    : null;

                if (elementType == null)
                {
                    continue;
                }

                var keys = new List<string>(block.Value.Keys);
                foreach (var key in keys)
                {
                    var propertyType = elementType.CompositionPropertyTypes.FirstOrDefault(x => x.Alias.InvariantEquals(key));
                    if (propertyType == null)
                    {
                        continue;
                    }

                    var propertyEditor = _propertyEditors[propertyType.PropertyEditorAlias];
                    if (propertyEditor == null)
                    {
                        continue;
                    }

                    var configuration = _dataTypeService.GetDataType(propertyType.DataTypeId).Configuration;
                    var contentPropertyData = new ContentPropertyData(block.Value[key], configuration)
                    {
                        ContentKey = block.Key,
                        PropertyTypeKey = propertyType.Key,
                        Files = new ContentPropertyFile[0]
                    };
                    var convertedValue = propertyEditor.GetValueEditor(configuration)?.FromEditor(contentPropertyData, block.Value[key]);

                    block.Value[key] = convertedValue != null
                        ? JToken.FromObject(convertedValue)
                        : null;
                }
            }

            return JsonConvert.SerializeObject(blocks);
        }
    }
}
