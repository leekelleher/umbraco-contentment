/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoContentPropertiesDataListSource : IDataListSource
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly Lazy<PropertyEditorCollection> _dataEditors;
        private readonly IIOHelper _ioHelper;
        private Dictionary<string, string> _icons;

        public UmbracoContentPropertiesDataListSource(
            IContentTypeService contentTypeService,
            Lazy<PropertyEditorCollection> dataEditors,
            IIOHelper ioHelper)
        {
            _contentTypeService = contentTypeService;
            _dataEditors = dataEditors;
            _ioHelper = ioHelper;
        }

        public string Name => "Umbraco Content Properties";

        public string Description => "Populate the data source using a Content Type's properties.";

        public string Icon => "icon-fa fa-tasks";

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

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
                        View = _ioHelper.ResolveRelativeOrVirtualUrl(ItemPickerDataListEditor.DataEditorViewPath),
                        Config = new Dictionary<string, object>
                        {
                            { "enableFilter", items.Count > 5 ? Constants.Values.True : Constants.Values.False },
                            { Constants.Conventions.ConfigurationFieldAliases.Items, items },
                            { "listType", "list" },
                            { Constants.Conventions.ConfigurationFieldAliases.OverlayView, _ioHelper.ResolveRelativeOrVirtualUrl(ItemPickerDataListEditor.DataEditorOverlayViewPath) },
                            { MaxItemsConfigurationField.MaxItems, 1 },
                        }
                    },
                    new ConfigurationField
                    {
                        Key = "includeName",
                        Name = "Include \"Name\" property?",
                        Description = "Select to include an option called \"Name\", for the content item's name.",
                        View = "boolean"
                    },
                    new ConfigurationField
                    {
                        Key = "sortAlphabetically",
                        Name = "Sort alphabetically?",
                        Description = "Select to sort the properties in alphabetical order.<br>By default, the order is defined by the order they appear on the document type.",
                        View = "boolean"
                    },
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
                UdiParser.TryParse(str, out GuidUdi udi) == true)
            {
                var contentType = _contentTypeService.Get(udi.Guid);
                if (contentType != null)
                {
                    if (_icons == null)
                    {
                        _icons = _dataEditors.Value.ToDictionary(x => x.Alias, x => x.Icon);
                    }

                    IEnumerable<DataListItem> addNameItem(bool add)
                    {
                        if (add == true)
                        {
                            yield return new DataListItem
                            {
                                Name = "Name",
                                Value = "name",
                                Description = "The name of the `IPublishedContent` item.",
                                Icon = UmbConstants.Icons.DataType
                            };
                        }
                    };

                    var includeName = config.GetValueAs("includeName", false);

                    var items = addNameItem(includeName)
                        .Union(contentType
                            .CompositionPropertyTypes
                            .Select(x => new DataListItem
                            {
                                Name = x.Name,
                                Value = x.Alias,
                                Description = x.PropertyEditorAlias,
                                Icon = _icons.ContainsKey(x.PropertyEditorAlias) == true
                                    ? _icons[x.PropertyEditorAlias]
                                    : UmbConstants.Icons.PropertyEditor,
                            }));

                    if (config.TryGetValueAs("sortAlphabetically", out bool sortAlphabetically) == true && sortAlphabetically == true)
                    {
                        return items.OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
                    }

                    return items;
                }
            }

            return Enumerable.Empty<DataListItem>();
        }
    }
}
