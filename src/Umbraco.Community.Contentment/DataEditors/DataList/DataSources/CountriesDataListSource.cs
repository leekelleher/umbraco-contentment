/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
#if NET472
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
#else
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class CountriesDataListSource : IDataListSource, IDataPickerSource
    {
        public string Name => ".NET Countries (ISO 3166-1)";

        public string Description => "All the countries available in the .NET Framework, (as installed on the web server).";

        public string Icon => "icon-globe-inverted-europe-africa";

        public string Group => Constants.Conventions.DataSourceGroups.DotNet;

        public IEnumerable<ConfigurationField> Fields => default;

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return GetRegions()
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

        public Task<PagedResult<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "")
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
                var results = new PagedResult<DataListItem>(items.Count(), pageNumber, pageSize)
                {
                    Items = items.Skip(offset).Take(pageSize)
                };

                return Task.FromResult(results);
            }

            return Task.FromResult(new PagedResult<DataListItem>(-1, pageNumber, pageSize));
        }

        private IEnumerable<RegionInfo> GetRegions()
        {
            return CultureInfo
               .GetCultures(CultureTypes.SpecificCultures)
#if NET7_0_OR_GREATER
                .Select(x => new RegionInfo(x.Name))
#else
               .Select(x => new RegionInfo(x.LCID))
#endif
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
