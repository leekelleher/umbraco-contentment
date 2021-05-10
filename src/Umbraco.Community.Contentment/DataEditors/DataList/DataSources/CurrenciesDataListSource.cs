/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Extensions;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Cms.Core.Composing.HideFromTypeFinder]
    public sealed class CurrenciesDataListSource : IDataListSource
    {
        public string Name => ".NET Currencies (ISO 4217)";

        public string Description => "All the currencies available in the .NET Framework, (as installed on the web server).";

        public string Icon => "icon-bills-euro";

        public string Group => default;

        public IEnumerable<ConfigurationField> Fields => default;

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Select(x => new RegionInfo(x.Name))
                .Where(x => x.GeoId != 39070) // Excludes "World/001"
                .DistinctBy(x => x.ISOCurrencySymbol)
                .OrderBy(x => x.ISOCurrencySymbol)
                .Select(x => new DataListItem
                {
                    Name = x.CurrencyEnglishName,
                    Value = x.ISOCurrencySymbol
                });
        }
    }
}
