/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class XmlSitemapChangeFrequencyDataListSource : IDataListSource
    {
        private readonly string[] _options;

        public XmlSitemapChangeFrequencyDataListSource()
        {
            _options = new[]
            {
                "always",
                "hourly",
                "daily",
                "weekly",
                "monthly",
                "yearly",
                "never",
            };
        }

        public string Name => "XML Sitemap: Change Frequency";

        public string Description => "Populate the data source using XML Sitemap change frequency values.";

        public string Icon => "icon-fa fa-signal";

        public string Group => Constants.Conventions.DataSourceGroups.Web;

        public IEnumerable<ConfigurationField> Fields => default;

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return _options.Select(x => new DataListItem { Name = x.ToFirstUpperInvariant(), Value = x });
        }
    }
}
