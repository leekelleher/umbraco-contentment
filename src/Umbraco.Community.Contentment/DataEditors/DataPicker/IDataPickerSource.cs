/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models;

namespace Umbraco.Community.Contentment.DataEditors
{
    public interface IDataPickerSource : IContentmentEditorItem
    {
        Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values);

        [Obsolete("Use `SearchAsync` with the additional `values` parameter instead. This method will be removed in Contentment 7.0.")]
        Task<PagedResult<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "");
    }

    // NOTE: Added as a separate interface, so not to break binary backwards-compatibility. [LK]
    public interface IDataPickerSource2 : IDataPickerSource
    {
        Task<PagedResult<DataListItem>> SearchAsync(
            Dictionary<string, object> config,
            int pageNumber = 1,
            int pageSize = 12,
            string query = "",
            IEnumerable<string>? values = null);
    }
}
