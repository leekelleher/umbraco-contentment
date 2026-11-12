// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Search.Indexing;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors;

/// <summary>
/// Indexes the name and description of each List Items entry as text. The value, icon and disabled flag are not indexed.
/// </summary>
public sealed class ListItemsPropertyValueHandler : IPropertyValueHandler
{
    private readonly IJsonSerializer _jsonSerializer;

    public ListItemsPropertyValueHandler(IJsonSerializer jsonSerializer)
        => _jsonSerializer = jsonSerializer;

    public bool CanHandle(IPropertyType propertyType)
        => propertyType.PropertyEditorAlias == ListItemsDataEditor.DataEditorAlias;

    public IEnumerable<IndexField> GetIndexFields(IProperty property, string? culture, string? segment, bool published, IContentBase contentContext)
    {
        var items = property.GetJsonValue<IEnumerable<DataListItem>>(culture, segment, published, _jsonSerializer);
        if (items is null)
        {
            return [];
        }

        var texts = new List<string>();
        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.Name) is false)
            {
                texts.Add(item.Name);
            }

            if (string.IsNullOrWhiteSpace(item.Description) is false)
            {
                texts.Add(item.Description);
            }
        }

        return texts.Count > 0
            ? [new IndexField(property.Alias, new IndexValue { Texts = texts }, culture, segment)]
            : [];
    }
}
