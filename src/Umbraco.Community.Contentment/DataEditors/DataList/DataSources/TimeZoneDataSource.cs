/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Umbraco.Core.Composing;

namespace Umbraco.Community.Contentment.DataEditors
{
    [HideFromTypeFinder]
    internal sealed class TimeZoneDataSource : IDataListSource
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
