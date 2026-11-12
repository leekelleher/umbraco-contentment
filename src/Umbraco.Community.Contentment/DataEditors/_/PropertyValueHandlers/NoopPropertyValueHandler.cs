// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Search.Indexing;

namespace Umbraco.Community.Contentment.DataEditors;

/// <summary>
/// Explicit "do not index" handler for Contentment editors whose value is not useful to search (display-only, or an icon alias).
/// </summary>
/// <remarks>
/// Registering a handler, rather than leaving the property unmatched, documents the intent and avoids the "no handler" debug log.
/// </remarks>
public sealed class NoopPropertyValueHandler : IPropertyValueHandler
{
    public bool CanHandle(IPropertyType propertyType)
        => propertyType.PropertyEditorAlias is
            IconPickerDataEditor.DataEditorAlias
            or NotesDataEditor.DataEditorAlias
            or EditorNotesDataEditor.DataEditorAlias;

    public IEnumerable<IndexField> GetIndexFields(IProperty property, string? culture, string? segment, bool published, IContentBase contentContext)
        => [];
}
