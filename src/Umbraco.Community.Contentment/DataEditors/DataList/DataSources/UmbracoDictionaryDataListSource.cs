/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Globalization;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoDictionaryDataListSource : DataListToDataPickerSourceBridge, IDataListSource
    {
        private readonly IDictionaryItemService _dictionaryItemService;

        public UmbracoDictionaryDataListSource(IDictionaryItemService dictionaryItemService)
        {
            _dictionaryItemService = dictionaryItemService;
        }

        public override string Name => "Umbraco Dictionary Items";

        public override string Description => "Select an Umbraco dictionary item to populate the data source with its child items.";

        public override string Icon => "icon-book-alt";

        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public override IEnumerable<ContentmentConfigurationField> Fields => new[]
        {
            new ContentmentConfigurationField
            {
                Key = "item",
                Name = "Dictionary item",
                Description = "Select a parent dictionary item to display the child items.",
                PropertyEditorUiAlias = DictionaryPickerDataEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { MaxItemsConfigurationField.MaxItems, 1 }
                }
            }
        };

        public override Dictionary<string, object>? DefaultValues => default;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            if (config.TryGetValueAs("item", out string? guid) == true &&
                string.IsNullOrWhiteSpace(guid) == false &&
                Guid.TryParse(guid, out var key) == true &&
                key.Equals(Guid.Empty) == false)
            {
                var parent = _dictionaryItemService.GetAsync(key).GetAwaiter().GetResult();
                if (parent is not null)
                {
                    var cultureName = CultureInfo.CurrentCulture.Name;

                    return _dictionaryItemService
                        .GetChildrenAsync(parent.Key).GetAwaiter().GetResult()
                        .OrderBy(x => x.ItemKey)
                        .Select(x => new DataListItem
                        {
                            Name = x.Translations.FirstOrDefault(t => t.LanguageIsoCode.InvariantEquals(cultureName) == true)?.Value ?? x.ItemKey,
                            Value = x.ItemKey,
                            Icon = Icon,
                            Description = x.ItemKey
                        });
                }
            }

            return Enumerable.Empty<DataListItem>();
        }
    }
}
