/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if NET472
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
#else
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
#endif

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
                .DistinctBy(x => x.DisplayName)
                .Where(x => x.TwoLetterISORegionName.Length == 2) // NOTE: Removes odd "countries" such as Caribbean (029), Europe (150), Latin America (419) and World (001).
                .OrderBy(x => x.DisplayName)
                .Select(x => new DataListItem
                {
                    Name = x.DisplayName,
                    Value = x.TwoLetterISORegionName
                });
        }
    }
}
