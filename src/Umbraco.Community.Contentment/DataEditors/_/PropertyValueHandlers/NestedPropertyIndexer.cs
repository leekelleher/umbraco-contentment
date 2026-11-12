// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using System.Text.Json;
using System.Text.Json.Nodes;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Search.Indexing;
using Umbraco.Cms.Core.Search.Indexing.Collection;

namespace Umbraco.Community.Contentment.DataEditors;

/// <summary>
/// Shared by the editors that nest other properties (Content Blocks, Input List).
/// </summary>
/// <remarks>
/// Based on the nested loop of Umbraco's `BlockEditorPropertyValueHandler`, minus its culture/segment variation,
/// block exposure and external element handling, none of which apply to the nested values of these editors.
/// </remarks>
internal static class NestedPropertyIndexer
{
    /// <summary>
    /// Gets the index values of one nested property value, by delegating to the handler of its property type.
    /// </summary>
    /// <remarks>
    /// The nested values are stored inside the parent property's value, so the property type should be invariant
    /// and not distinguish published values (<see cref="IPropertyType.SupportsPublishing"/> is <c>false</c>).
    /// </remarks>
    public static IEnumerable<IndexValue> GetIndexValues(
        IPropertyType nestedPropertyType,
        object? value,
        bool published,
        IContentBase contentContext,
        PropertyValueHandlerCollection propertyValueHandlers)
    {
        var handler = propertyValueHandlers.GetPropertyValueHandler(nestedPropertyType);
        if (handler is null)
        {
            return [];
        }

        var property = new Property(nestedPropertyType);
        property.SetValue(Normalize(value));

        return handler
            .GetIndexFields(property, null, null, published, contentContext)
            .Select(x => x.Value);
    }

    /// <summary>
    /// Converts a deserialized JSON value back to the form a property stores, i.e. a string for JSON/text editors, an integer for integer editors.
    /// </summary>
    private static object? Normalize(object? value) => value switch
    {
        JsonElement element => Normalize(element),
        JsonNode node => Normalize(JsonSerializer.SerializeToElement(node)),
        _ => value,
    };

    private static object? Normalize(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.String => element.GetString(),
        JsonValueKind.Number when element.TryGetInt32(out var i) => i,
        JsonValueKind.Number when element.TryGetInt64(out var l) => l,
        JsonValueKind.Number when element.TryGetDecimal(out var d) => d,
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null or JsonValueKind.Undefined => null,
        _ => element.GetRawText(),
    };
}
