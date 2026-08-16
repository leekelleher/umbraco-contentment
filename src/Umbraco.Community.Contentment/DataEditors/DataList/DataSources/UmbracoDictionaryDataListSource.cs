/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoDictionaryDataListSource : DataListToDataPickerSourceBridge, IContentmentDataSource
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        private readonly IDictionaryItemService _dictionaryItemService;
        private readonly GlobalSettings _globalSettings;

        public UmbracoDictionaryDataListSource(
            IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
            IDictionaryItemService dictionaryItemService,
            IOptions<GlobalSettings> globalSettings)
        {
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            _dictionaryItemService = dictionaryItemService;
            _globalSettings = globalSettings.Value;
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

        public override OverlaySize OverlaySize => OverlaySize.Medium;

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
                    var userLanguage = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser?.Language;
                    var cultureName = string.IsNullOrWhiteSpace(userLanguage) == false
                        ? userLanguage
                        : _globalSettings.DefaultUILanguage;
                    var languageName = cultureName.Split('-')[0];

                    return _dictionaryItemService
                        .GetChildrenAsync(parent.Key).GetAwaiter().GetResult()
                        .OrderBy(x => x.ItemKey)
                        .Select(x => new DataListItem
                        {
                            Name = GetTranslatedValue(x, cultureName, languageName) ?? x.ItemKey,
                            Value = x.ItemKey,
                            Icon = Icon,
                            Description = x.ItemKey
                        });
                }
            }

            return Enumerable.Empty<DataListItem>();
        }

        private static string? GetTranslatedValue(IDictionaryItem item, string cultureName, string languageName)
        {
            var translations = item.Translations
                .Where(x => string.IsNullOrWhiteSpace(x.Value) == false)
                .ToList();

            return translations.FirstOrDefault(x => x.LanguageIsoCode.InvariantEquals(cultureName) == true)?.Value
                ?? translations.FirstOrDefault(x => x.LanguageIsoCode.Split('-')[0].InvariantEquals(languageName) == true)?.Value
                ?? translations.FirstOrDefault()?.Value;
        }
    }
}
