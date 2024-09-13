/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Globalization;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class LanguagesDataListSource : IDataListSource, IDataPickerSource
    {
        public string Name => ".NET Languages (ISO 639-1)";

        public string Description => "All the languages available in the .NET Framework, (as installed on the web server).";

        public string Icon => "icon-fa-language";

        public string Group => Constants.Conventions.DataSourceGroups.DotNet;

        public IEnumerable<ContentmentConfigurationField> Fields => Enumerable.Empty<ContentmentConfigurationField>();

        public Dictionary<string, object>? DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return GetCultures()
                .OrderBy(x => x.EnglishName)
                .Select(ToDataListItem);
        }

        public Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
        {
            if (values?.Any() == true)
            {
                var lookup = GetCultures().ToLookup(x => x.TwoLetterISOLanguageName, StringComparer.InvariantCultureIgnoreCase);

                return Task.FromResult(values
                    .Where(x => lookup.Contains(x) == true)
                    .SelectMany(x => lookup[x])
                    .Select(ToDataListItem));
            }

            return Task.FromResult(Enumerable.Empty<DataListItem>());
        }

        public Task<PagedResult<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "")
        {
            var items = default(IEnumerable<DataListItem>);

            if (string.IsNullOrWhiteSpace(query) == true)
            {
                items = GetItems(config);
            }
            else
            {
                items = GetCultures()
                    .Where(x => x.EnglishName.InvariantContains(query) == true || x.TwoLetterISOLanguageName.InvariantStartsWith(query) == true)
                    .OrderBy(x => x.EnglishName)
                    .Select(ToDataListItem);
            }

            if (items?.Any() == true)
            {
                var offset = (pageNumber - 1) * pageSize;
                var results = new PagedResult<DataListItem>(items.Count(), pageNumber, pageSize)
                {
                    Items = items.Skip(offset).Take(pageSize)
                };

                return Task.FromResult(results);
            }

            return Task.FromResult(new PagedResult<DataListItem>(-1, pageNumber, pageSize));
        }

        private static IEnumerable<CultureInfo> GetCultures()
        {
            return CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .DistinctBy(x => x.TwoLetterISOLanguageName)
                // NOTE: Removes any odd languages.
                .Where(x => x.TwoLetterISOLanguageName.Length == 2);
        }

        private DataListItem ToDataListItem(CultureInfo culture)
        {
            return new DataListItem
            {
                Name = culture.EnglishName,
                Value = culture.TwoLetterISOLanguageName,
                Icon = "icon-umb-translation",
                Description = culture.TwoLetterISOLanguageName,
            };
        }
    }
}
