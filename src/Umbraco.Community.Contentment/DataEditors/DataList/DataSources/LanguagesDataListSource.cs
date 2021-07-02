/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class LanguagesDataListSource : IDataListSource
    {
        public string Name => ".NET Languages (ISO 639)";

        public string Description => "All the languages available in the .NET Framework, (as installed on the web server).";

        public string Icon => "icon-fa fa-language";

        public string Group => Constants.Conventions.DataSourceGroups.DotNet;

        public IEnumerable<ConfigurationField> Fields => default;

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .DistinctBy(x => x.DisplayName)
                .OrderBy(x => x.DisplayName)
                .Select(x => new DataListItem
                {
                    Name = x.DisplayName,
                    Value = x.TwoLetterISOLanguageName
                });
        }
    }
}
