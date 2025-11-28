/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Globalization;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class CountriesDataListSource : IContentmentDataSource, IDataPickerSource
    {
        public string Name => ".NET Countries (ISO 3166-1)";

        public string Description => "All the countries available in the .NET Framework, (as installed on the web server).";

        public string Icon => "icon-fa-earth-africa";

        public string Group => Constants.Conventions.DataSourceGroups.DotNet;

        public IEnumerable<ContentmentConfigurationField> Fields => Enumerable.Empty<ContentmentConfigurationField>();

        public Dictionary<string, object>? DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return CountriesDataListSource.GetRegions()
                .OrderBy(x => x.EnglishName)
                .Select(ToDataListItem);
        }

        public Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
        {
            if (values?.Any() == true)
            {
                return Task.FromResult(values
                    .Select(x => new RegionInfo(x))
                    .Select(ToDataListItem));
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
                items = GetRegions()
                    .Where(x => x.EnglishName.InvariantContains(query) == true || x.TwoLetterISORegionName.InvariantStartsWith(query) == true)
                    .OrderBy(x => x.EnglishName)
                    .Select(ToDataListItem);
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

        private static IEnumerable<RegionInfo> GetRegions()
        {
            return CultureInfo
               .GetCultures(CultureTypes.SpecificCultures)
               .Select(x => new RegionInfo(x.Name))
               .DistinctBy(x => x.TwoLetterISORegionName)
               // NOTE: Removes odd "countries" such as Caribbean (029), Europe (150), Latin America (419) and World (001).
               .Where(x => x.TwoLetterISORegionName.Length == 2);
        }

        private DataListItem ToDataListItem(RegionInfo region)
        {
            return new DataListItem
            {
                Name = region.EnglishName,
                Value = region.TwoLetterISORegionName,
                Icon = "icon-globe",
                Description = region.TwoLetterISORegionName,
                //Properties = new Dictionary<string, object>
                //{
                //    { "image", $"https://flagcdn.com/256x192/{region.TwoLetterISORegionName.ToLowerInvariant()}.png" },
                //},
            };
        }
    }
}
