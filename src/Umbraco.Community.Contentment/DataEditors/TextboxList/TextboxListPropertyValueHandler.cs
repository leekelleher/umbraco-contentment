// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using System.Text.Json.Nodes;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Search.Indexing;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors;

/// <summary>
/// Indexes the typed values of a Textbox List as text. The keys are identifiers, so they are not indexed.
/// </summary>
public sealed class TextboxListPropertyValueHandler : IPropertyValueHandler
{
    public bool CanHandle(IPropertyType propertyType)
        => propertyType.PropertyEditorAlias == TextboxListDataEditor.DataEditorAlias;

    public IEnumerable<IndexField> GetIndexFields(IProperty property, string? culture, string? segment, bool published, IContentBase contentContext)
    {
        if (property.GetValue(culture, segment, published) is not string value ||
            string.IsNullOrWhiteSpace(value) ||
            value.DetectIsJson() is false ||
            JsonNode.Parse(value) is not JsonObject obj)
        {
            return [];
        }

        var texts = obj
            .Select(x => x.Value?.ToString())
            .Where(x => string.IsNullOrWhiteSpace(x) is false)
            .Cast<string>()
            .ToArray();

        return texts.Length > 0
            ? [new IndexField(property.Alias, new IndexValue { Texts = texts }, culture, segment)]
            : [];
    }
}
