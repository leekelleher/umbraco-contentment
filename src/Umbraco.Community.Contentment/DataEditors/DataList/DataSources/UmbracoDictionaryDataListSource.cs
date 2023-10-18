/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Globalization;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoDictionaryDataListSource : IDataListSource
    {
        private readonly ILocalizationService _localizationService;
        private readonly IIOHelper _ioHelper;

        public UmbracoDictionaryDataListSource(
            ILocalizationService localizationService,
            IIOHelper ioHelper)
        {
            _localizationService = localizationService;
            _ioHelper = ioHelper;
        }

        public string Name => "Umbraco Dictionary Items";

        public string Description => "Select an Umbraco dictionary item to populate the data source with its child items.";

        public string Icon => "icon-book-alt";

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "item",
                Name = "Dictionary item",
                Description = "Select a parent dictionary item to display the child items.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(DictionaryPickerDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { MaxItemsConfigurationField.MaxItems, 1 }
                }
            }
        };

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            if (config.TryGetValueAs("item", out JArray array) == true &&
                array.Count > 0 &&
                array[0] is JObject dictItem)
            {
                var parent = default(IDictionaryItem);

                if (dictItem.Value<string>("key") is string guid && Guid.TryParse(guid, out var key) == true && key.Equals(Guid.Empty) == false)
                {
                    parent = _localizationService.GetDictionaryItemById(key);
                }
                else if (dictItem.Value<int>("id") is int id && id > 0)
                {
                    // NOTE: Fallback on the `int` ID (for backwards-compatibility)
                    parent = _localizationService.GetDictionaryItemById(id);
                }

                if (parent != null)
                {
                    var cultureName = CultureInfo.CurrentCulture.Name;

                    return _localizationService
                        .GetDictionaryItemChildren(parent.Key)
                        .OrderBy(x => x.ItemKey)
                        .Select(x => new DataListItem
                        {
                            Name = x.Translations.FirstOrDefault(t => t.Language.IsoCode.InvariantEquals(cultureName) == true || t.Language.IsDefault == true)?.Value ?? x.ItemKey,
                            Value = x.ItemKey,
                            Icon = this.Icon,
                            Description = x.ItemKey
                        });
                }
            }

            return Enumerable.Empty<DataListItem>();
        }
    }
}
