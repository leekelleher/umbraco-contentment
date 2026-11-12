// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using Umbraco.Cms.Core.Search.Indexing;

namespace Umbraco.Community.Contentment.DataEditors;

/// <summary>
/// Lets a data source decide how one of its stored values is indexed for search (Umbraco 19 Search).
/// </summary>
/// <remarks>
/// Used by the Data List and Data Picker editors.
/// A data source that does not implement this interface has each stored value indexed as a keyword, as-is.
/// </remarks>
public interface IDataSourceValueIndexer : IContentmentDataSource
{
    /// <summary>
    /// Gets the index value for a single stored value, or <c>null</c> when the value should not be indexed.
    /// </summary>
    /// <param name="config">The data source configuration.</param>
    /// <param name="value">The stored value (one item of the Data List, or Data Picker, value).</param>
    IndexValue? GetIndexValue(Dictionary<string, object>? config, string value);
}
