/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;

namespace Umbraco.Community.Contentment.DataEditors
{
#if !DEBUG
    // TODO: Consider whether this data source is necessary. [LK:2019-09-05]
    [global::Umbraco.Core.Composing.HideFromTypeFinder]
#endif
    public class TimeZoneDataSource : IDataListSource
    {
        public string Name => "Time zones";

        public string Description => "Data source for all the time zones.";

        public string Icon => "icon-globe";

        public IEnumerable<DataListItem> GetItems()
        {
            var items = new List<DataListItem>();

            foreach (var timezone in TimeZoneInfo.GetSystemTimeZones())
            {
                items.Add(new DataListItem
                {
                    Name = timezone.DisplayName,
                    Value = timezone.BaseUtcOffset.ToString()
                });
            }

            return items;
        }
    }
}
