/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Globalization;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class LanguagesDataListSource : IContentmentDataSource, IDataPickerSource
    {
        private const string _displayModeEnglishName = "englishName";
        private const string _displayModeNativeName = "nativeName";
        private const string _displayModeBackofficeUserLanguage = "backofficeUserLanguage";

        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

        public LanguagesDataListSource(IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
        {
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        }

        public string Name => ".NET Languages (ISO 639-1)";

        public string Description => "All the languages available in the .NET Framework, (as installed on the web server).";

        public string Icon => "icon-fa-language";

        public string Group => Constants.Conventions.DataSourceGroups.DotNet;

        public IEnumerable<ContentmentConfigurationField> Fields => new[]
        {
            new ContentmentConfigurationField
            {
                Key = "displayMode",
                Name = "Display mode",
                Description = "Choose how each language name is displayed in the list.",
                PropertyEditorUiAlias = RadioButtonListDataListEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "English name", Value = _displayModeEnglishName, Description = "e.g. \"German\", \"Japanese\"." },
                            new DataListItem { Name = "Native name", Value = _displayModeNativeName, Description = "Each language in its own language, e.g. \"Deutsch\", \"日本語\"." },
                            new DataListItem { Name = "Backoffice user language", Value = _displayModeBackofficeUserLanguage, Description = "Localised in the current backoffice user's language, e.g. \"allemand\", \"japonais\" for a French user." },
                        }
                    },
                }
            }
        };

        public Dictionary<string, object>? DefaultValues => new()
        {
            { "displayMode", _displayModeEnglishName },
        };

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var displayMode = GetDisplayMode(config);

            return GetCultures()
                .Select(x => (Culture: x, Name: GetDisplayName(x, displayMode)))
                .OrderBy(x => x.Name, StringComparer.CurrentCultureIgnoreCase)
                .Select(x => ToDataListItem(x.Culture, x.Name));
        }

        public Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
        {
            if (values?.Any() == true)
            {
                var displayMode = GetDisplayMode(config);
                var lookup = GetCultures().ToLookup(x => x.TwoLetterISOLanguageName, StringComparer.InvariantCultureIgnoreCase);

                return Task.FromResult(values
                    .Where(x => lookup.Contains(x) == true)
                    .SelectMany(x => lookup[x])
                    .Select(x => ToDataListItem(x, GetDisplayName(x, displayMode))));
            }

            return Task.FromResult(Enumerable.Empty<DataListItem>());
        }

        public Task<PagedViewModel<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "")
        {
            var items = default(IEnumerable<DataListItem>);

            if (string.IsNullOrWhiteSpace(query) == true)
            {
                items = GetItems(config);
            }
            else
            {
                var displayMode = GetDisplayMode(config);

                items = GetCultures()
                    .Select(x => (Culture: x, Name: GetDisplayName(x, displayMode)))
                    .Where(x => x.Name.InvariantContains(query) == true
                        || x.Culture.EnglishName.InvariantContains(query) == true
                        || x.Culture.TwoLetterISOLanguageName.InvariantStartsWith(query) == true)
                    .OrderBy(x => x.Name, StringComparer.CurrentCultureIgnoreCase)
                    .Select(x => ToDataListItem(x.Culture, x.Name));
            }

            if (items?.Any() == true)
            {
                var offset = (pageNumber - 1) * pageSize;
                var results = new PagedViewModel<DataListItem>
                {
                    Items = items.Skip(offset).Take(pageSize),
                    Total = pageSize > 0 ? (long)Math.Ceiling(items.Count() / (decimal)pageSize) : 1,
                };

                return Task.FromResult(results);
            }

            return Task.FromResult(PagedViewModel<DataListItem>.Empty());
        }

        private static IEnumerable<CultureInfo> GetCultures()
        {
            return CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .DistinctBy(x => x.TwoLetterISOLanguageName)
                // NOTE: Removes any odd languages.
                .Where(x => x.TwoLetterISOLanguageName.Length == 2);
        }

        private static string GetDisplayMode(Dictionary<string, object>? config)
        {
            if (config?.TryGetValueAs("displayMode", out string? displayMode) == true &&
                string.IsNullOrWhiteSpace(displayMode) == false)
            {
                return displayMode;
            }

            return _displayModeEnglishName;
        }

        private string GetDisplayName(CultureInfo culture, string displayMode) => displayMode switch
        {
            _displayModeNativeName => culture.NativeName,
            _displayModeBackofficeUserLanguage => GetBackofficeUserDisplayName(culture),
            _ => culture.EnglishName,
        };

        private string GetBackofficeUserDisplayName(CultureInfo culture)
        {
            var userLanguage = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser?.Language;

            if (string.IsNullOrWhiteSpace(userLanguage) == true)
            {
                return culture.EnglishName;
            }

            try
            {
                var userCulture = CultureInfo.GetCultureInfo(userLanguage);
                var previous = CultureInfo.CurrentUICulture;
                try
                {
                    CultureInfo.CurrentUICulture = userCulture;
                    return culture.DisplayName;
                }
                finally
                {
                    CultureInfo.CurrentUICulture = previous;
                }
            }
            catch (CultureNotFoundException)
            {
                return culture.EnglishName;
            }
        }

        private static DataListItem ToDataListItem(CultureInfo culture, string name)
        {
            return new DataListItem
            {
                Name = name,
                Value = culture.TwoLetterISOLanguageName,
                Icon = "icon-fa-language",
                Description = culture.TwoLetterISOLanguageName,
            };
        }
    }
}
