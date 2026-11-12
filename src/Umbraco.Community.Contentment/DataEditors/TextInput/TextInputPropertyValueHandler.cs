// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Search.Indexing;

namespace Umbraco.Community.Contentment.DataEditors;

public sealed class TextInputPropertyValueHandler : IPropertyValueHandler
{
    public bool CanHandle(IPropertyType propertyType)
        => propertyType.PropertyEditorAlias == TextInputDataEditor.DataEditorAlias;

    public IEnumerable<IndexField> GetIndexFields(IProperty property, string? culture, string? segment, bool published, IContentBase contentContext)
        => property.GetValue(culture, segment, published) is string value && string.IsNullOrWhiteSpace(value) is false
            ? [new IndexField(property.Alias, new IndexValue { Texts = [value] }, culture, segment)]
            : [];
}
