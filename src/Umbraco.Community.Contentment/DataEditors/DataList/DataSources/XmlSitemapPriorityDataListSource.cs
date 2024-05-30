/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Linq;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class XmlSitemapPriorityDataListSource : DataListToDataPickerSourceBridge, IDataListSource
    {
        public override string Name => "XML Sitemap: Priority";

        public override string Description => "Populate the data source using XML Sitemap priority values.";

        public override string Icon => "icon-fa fa-exclamation-circle";

        public override string Group => Constants.Conventions.DataSourceGroups.Web;

        public override IEnumerable<ContentmentConfigurationField> Fields => Enumerable.Empty<ContentmentConfigurationField>();

        public override Dictionary<string, object>? DefaultValues => default;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            for (var i = 0.0D; i <= 1.0D; i += 0.1D)
            {
                yield return new DataListItem { Name = i.ToString("0.0"), Value = i.ToString("0.0") };
            }
        }
    }
}
