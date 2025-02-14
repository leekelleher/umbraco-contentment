/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoContentPropertiesDataListSource : DataListToDataPickerSourceBridge, IContentmentDataSource
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly Lazy<PropertyEditorCollection> _dataEditors;
        private Dictionary<string, string>? _icons;

        public UmbracoContentPropertiesDataListSource(
            IContentTypeService contentTypeService,
            Lazy<PropertyEditorCollection> dataEditors)
        {
            _contentTypeService = contentTypeService;
            _dataEditors = dataEditors;
        }

        public override string Name => "Umbraco Content Properties";

        public override string Description => "Populate the data source using a Content Type's properties.";

        public override string Icon => "icon-fa-list-check";

        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public override IEnumerable<ContentmentConfigurationField> Fields
        {
            get
            {
                //var items = _contentTypeService
                //    .GetAll()
                //    .Select(x => new DataListItem
                //    {
                //        Icon = x.Icon,
                //        Description = x.Description,
                //        Name = x.Name,
                //        Value = Udi.Create(UmbConstants.UdiEntityType.DocumentType, x.Key).ToString(),
                //    })
                //    .ToList();

                return new ContentmentConfigurationField[]
                {
                    new ContentmentConfigurationField
                    {
                        Key = "contentType",
                        Name = "Content Type",
                        Description = "Select a Content Type to list the properties from.",
                        PropertyEditorUiAlias = "Umb.PropertyEditorUi.DocumentTypePicker",
                        Config = new Dictionary<string, object>
                        {
                            // { "enableFilter", items.Count > 5 ? true : false },
                            // { Constants.Conventions.ConfigurationFieldAliases.Items, items },
                            // { "listType", "list" },
                            // { Constants.Conventions.ConfigurationFieldAliases.OverlayView, _ioHelper.ResolveRelativeOrVirtualUrl(ItemPickerDataListEditor.DataEditorOverlayViewPath) ?? string.Empty },
                            // { MaxItemsConfigurationField.MaxItems, 1 },
                            { "validationLimit", new { min = 0, max = 1 } },
                            { "onlyElementTypes", false },
                            { "showOpenButton", false },
                        }
                    },
                    new ContentmentConfigurationField
                    {
                        Key = "includeName",
                        Name = "Include \"Name\" property?",
                        Description = "Select to include an option called \"Name\", for the content item's name.",
                        PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
                    },
                    new ContentmentConfigurationField
                    {
                        Key = "sortAlphabetically",
                        Name = "Sort alphabetically?",
                        Description = "Select to sort the properties in alphabetical order.<br>By default, the order is defined by the order they appear on the document type.",
                        PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
                    },
                };
            }
        }

        public override Dictionary<string, object>? DefaultValues => default;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            if (config.TryGetValueAs("contentType", out string? str) == true &&
                string.IsNullOrWhiteSpace(str) == false &&
                Guid.TryParse(str, out var guid) == true &&
                guid.Equals(Guid.Empty) == false)
            {
                var contentType = _contentTypeService.Get(guid);
                if (contentType != null)
                {
                    _icons ??= _dataEditors.Value.ToDictionary(x => x.Alias, x => "icon-document");

                    static IEnumerable<DataListItem> addNameItem(bool add)
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
