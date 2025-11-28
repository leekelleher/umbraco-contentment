// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

namespace Umbraco.Community.Contentment.DataEditors;

public interface IContentmentDataSource : IContentmentEditorItem
{
    public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config);
}
