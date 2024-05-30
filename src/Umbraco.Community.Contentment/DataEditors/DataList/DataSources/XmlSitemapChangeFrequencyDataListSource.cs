/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class XmlSitemapChangeFrequencyDataListSource : DataListToDataPickerSourceBridge, IDataListSource
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

        public override string Name => "XML Sitemap: Change Frequency";

        public override string Description => "Populate the data source using XML Sitemap change frequency values.";

        public override string Icon => "icon-fa fa-signal";

        public override string Group => Constants.Conventions.DataSourceGroups.Web;

        public override IEnumerable<ContentmentConfigurationField> Fields => Enumerable.Empty<ContentmentConfigurationField>();

        public override Dictionary<string, object>? DefaultValues => default;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return _options.Select(x => new DataListItem { Name = x.ToFirstUpperInvariant(), Value = x });
        }
    }
}
