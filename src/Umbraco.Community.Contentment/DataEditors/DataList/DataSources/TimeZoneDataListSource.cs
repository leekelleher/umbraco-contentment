/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class TimeZoneDataListSource : IDataListSource, IDataPickerSource, IDataSourceValueConverter
    {
        public string Name => ".NET Time Zones (UTC)";

        public string Description => "All the time zones available in the .NET Framework.";

        public string Icon => "icon-globe";

        public string Group => Constants.Conventions.DataSourceGroups.DotNet;

        public IEnumerable<ConfigurationField> Fields => Enumerable.Empty<ConfigurationField>();

        public Dictionary<string, object>? DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return TimeZoneInfo
                .GetSystemTimeZones()
                .Select(ToDataListItem);
        }

        public Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
        {
            if (values?.Any() == true)
            {
                return Task.FromResult(values
                    .Select(x => TimeZoneInfo.FindSystemTimeZoneById(x))
                    .Select(ToDataListItem));
            }

            return Task.FromResult(Enumerable.Empty<DataListItem>());
        }

        public Task<PagedResult<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "")
        {
            var items = default(IEnumerable<DataListItem>);

            if (string.IsNullOrWhiteSpace(query) == true)
            {
                items = GetItems(config);
            }
            else
            {
                items = GetItems(config)
                    .Where(x => x.Name.InvariantContains(query) == true || x.Value.InvariantStartsWith(query) == true);
            }

            if (items?.Any() == true)
            {
                var offset = (pageNumber - 1) * pageSize;
                var results = new PagedResult<DataListItem>(items.Count(), pageNumber, pageSize)
                {
                    Items = items.Skip(offset).Take(pageSize)
                };

                return Task.FromResult(results);
            }

            return Task.FromResult(new PagedResult<DataListItem>(-1, pageNumber, pageSize));
        }

        private DataListItem ToDataListItem(TimeZoneInfo info)
        {
            return new DataListItem
            {
                Name = info.DisplayName,
                Value = info.Id,
                Icon = "icon-time",
                Description = info.Id,
            };
        }

        public Type GetValueType(Dictionary<string, object> config) => typeof(TimeZoneInfo);

        public object ConvertValue(Type type, string value) => TimeZoneInfo.FindSystemTimeZoneById(value);
    }
}
