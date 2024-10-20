/* Copyright © 2024 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public abstract class DataListToDataPickerSourceBridge : IDataListSource, IDataPickerSource
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract string Icon { get; }

        public abstract string Group { get; }

        public abstract IEnumerable<ContentmentConfigurationField> Fields { get; }

        public abstract Dictionary<string, object>? DefaultValues { get; }

        public abstract OverlaySize OverlaySize { get; }

        public abstract IEnumerable<DataListItem> GetItems(Dictionary<string, object> config);

        public virtual Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
        {
            var items = GetItems(config);

            if (items?.Any() == true)
            {
                return Task.FromResult(items.Where(x => values.Contains(x.Value) == true));
            }

            return Task.FromResult(Enumerable.Empty<DataListItem>());
        }

        public virtual Task<PagedViewModel<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "")
        {
            var items = default(IEnumerable<DataListItem>);

            if (string.IsNullOrWhiteSpace(query) == true)
            {
                items = GetItems(config);
            }
            else
            {
                items = GetItems(config)?.Where(x => x.Name?.InvariantContains(query) == true || x.Value?.InvariantStartsWith(query) == true);
            }

            if (items?.Any() == true)
            {
                var offset = (pageNumber - 1) * pageSize;
                return Task.FromResult(new PagedViewModel<DataListItem>
                {
                    Items = items.Skip(offset).Take(pageSize),
                    Total = pageSize > 0 ? (long)Math.Ceiling(items.Count() / (decimal)pageSize) : 1,
                });
            }

            return Task.FromResult(PagedViewModel<DataListItem>.Empty());
        }
    }
}
