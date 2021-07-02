/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class CountriesDataListSource : IDataListSource
    {
        public string Name => ".NET Countries (ISO 3166)";

        public string Description => "All the countries available in the .NET Framework, (as installed on the web server).";

        public string Icon => "icon-globe-inverted-europe-africa";

        public string Group => Constants.Conventions.DataSourceGroups.DotNet;

        public IEnumerable<ConfigurationField> Fields => default;

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Select(x => new RegionInfo(x.Name))
                .Where(x => x.GeoId != 39070) // Excludes "World/001"
                .DistinctBy(x => x.TwoLetterISORegionName)
                .OrderBy(x => x.EnglishName)
                .Select(x => new DataListItem
                {
                    Name = x.EnglishName,
                    Value = x.TwoLetterISORegionName
                });
        }
    }
}
