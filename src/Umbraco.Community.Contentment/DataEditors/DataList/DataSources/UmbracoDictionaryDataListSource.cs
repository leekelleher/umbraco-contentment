/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Core.Composing.HideFromTypeFinder]
    public sealed class UmbracoDictionaryDataListSource : IDataListSource
    {
        private readonly ILocalizationService _localizationService;

        public UmbracoDictionaryDataListSource(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
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
                View = DictionaryPickerDataEditor.DataEditorViewPath,
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
                array[0].Value<int>("id") is int id &&
                id > 0)
            {
                var parent = _localizationService.GetDictionaryItemById(id);

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
