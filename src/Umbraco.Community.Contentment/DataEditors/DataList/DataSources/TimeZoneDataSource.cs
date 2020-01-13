/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Core.Composing.HideFromTypeFinder]
    public sealed class TimeZoneDataSource : IDataListSource
    {
        public string Name => "Time zones";

        public string Description => "Data source for all the time zones.";

        public string Icon => "icon-globe";

        public IEnumerable<ConfigurationField> Fields => default;

        public Dictionary<string, object> DefaultValues => default;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
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
