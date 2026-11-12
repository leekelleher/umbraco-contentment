// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.Search.Indexing;

namespace Umbraco.Community.Contentment.DataEditors;

internal static class UdiIndexValue
{
    /// <summary>
    /// Indexes a stored UDI as its Guid keyword, the same as Umbraco's own pickers.
    /// </summary>
    public static IndexValue? Create(string value)
        => UdiParser.TryParse(value, out GuidUdi? udi) && udi!.Guid != Guid.Empty
            ? new IndexValue { Keywords = [udi.Guid.AsKeyword()] }
            : null;
}
