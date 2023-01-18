/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Umbraco.Community.Contentment.DataEditors
{
    public interface IDataPickerSource : IContentmentEditorItem
    {
        Task<IEnumerable<DataPickerItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values);

        Task<IEnumerable<DataPickerItem>> SearchAsync(Dictionary<string, object> config, out int totalPages, int pageNumber = 1, int pageSize = 12, string query = "");
    }
}
