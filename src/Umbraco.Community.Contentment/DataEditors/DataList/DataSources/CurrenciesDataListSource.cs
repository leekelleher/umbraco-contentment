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
    public sealed class CurrenciesDataListSource : IDataListSource
    {
        public string Name => ".NET Currencies (ISO 4217)";

        public string Description => "All the currencies available in the .NET Framework, (as installed on the web server).";

        public string Icon => "icon-bills-euro";

        public string Group => Constants.Conventions.DataSourceGroups.DotNet;

        public IEnumerable<ConfigurationField> Fields => default;

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
#if NET7_0_OR_GREATER
                .Select(x => new RegionInfo(x.Name))
#else
                .Select(x => new RegionInfo(x.LCID))
#endif
                .DistinctBy(x => x.ISOCurrencySymbol)
                .Where(x => x.ISOCurrencySymbol.Length == 3) // NOTE: Removes any odd currencies.
                .OrderBy(x => x.CurrencyEnglishName)
                .Select(x => new DataListItem
                {
                    Name = x.CurrencyEnglishName,
                    Value = x.ISOCurrencySymbol
                });
        }
    }
}
