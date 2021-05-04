/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using UmbConstants = Umbraco.Cms.Core.Constants;
using Umbraco.Cms.Core;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Cms.Core.Composing.HideFromTypeFinder]
    public sealed class UmbracoContentPropertiesDataListSource : IDataListSource
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly Lazy<PropertyEditorCollection> _dataEditors;
        private Dictionary<string, string> _icons;

        public UmbracoContentPropertiesDataListSource(IContentTypeService contentTypeService, Lazy<PropertyEditorCollection> dataEditors)
        {
            _contentTypeService = contentTypeService;
            _dataEditors = dataEditors;
        }

        public string Name => "Umbraco Content Properties";

        public string Description => "Populate a data source using a Content Type's properties.";

        public string Icon => "icon-umbraco";

        public string Group => default;

        public IEnumerable<ConfigurationField> Fields
        {
            get
            {
                var items = _contentTypeService
                    .GetAll()
                    .Select(x => new DataListItem
                    {
                        Icon = x.Icon,
                        Description = x.Description,
                        Name = x.Name,
                        Value = Udi.Create(UmbConstants.UdiEntityType.DocumentType, x.Key).ToString(),
                    })
                    .ToList();

                return new ConfigurationField[]
                {
                    new ConfigurationField
                    {
                        Key = "contentType",
                        Name = "Content Type",
                        Description = "Select a Content Type to list the properties from.",
                        View = ItemPickerDataListEditor.DataEditorViewPath,
                        Config = new Dictionary<string, object>
                        {
                            { "enableFilter", items.Count > 5 ? Constants.Values.True : Constants.Values.False },
                            { "items", items },
                            { "listType", "list" },
                            { "overlayView", IOHelper.ResolveUrl(ItemPickerDataListEditor.DataEditorOverlayViewPath) },
                            { "maxItems", 1 },
                        }
                    }
                };
            }
        }

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            if (config.TryGetValueAs("contentType", out JArray array) == true &&
                array.Count > 0 &&
                array[0].Value<string>() is string str &&
                string.IsNullOrWhiteSpace(str) == false &&
                GuidUdi.TryParse(str, out var udi) == true)
            {
                var contentType = _contentTypeService.Get(udi.Guid);
                if (contentType != null)
                {
                    if (_icons == null)
                    {
                        _icons = _dataEditors.Value.ToDictionary(x => x.Alias, x => x.Icon);
                    }

                    return contentType
                        .CompositionPropertyTypes
                        .Select(x => new DataListItem
                        {
                            Name = x.Name,
                            Value = x.Alias,
                            Description = x.PropertyEditorAlias,
                            Icon = _icons.ContainsKey(x.PropertyEditorAlias) == true
                                ? _icons[x.PropertyEditorAlias]
                                : UmbConstants.Icons.PropertyEditor,
                        });
                }
            }

            return Enumerable.Empty<DataListItem>();
        }
    }
}
