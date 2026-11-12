// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using System.Globalization;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Search.Indexing;

namespace Umbraco.Community.Contentment.DataEditors;

public sealed class BytesPropertyValueHandler : IPropertyValueHandler
{
    public bool CanHandle(IPropertyType propertyType)
        => propertyType.PropertyEditorAlias == BytesDataEditor.DataEditorAlias;

    public IEnumerable<IndexField> GetIndexFields(IProperty property, string? culture, string? segment, bool published, IContentBase contentContext)
        // NOTE: A bigint is stored as a string. It can overflow `IndexValue.Integers` (32-bit), so it is indexed as a decimal.
        => property.GetValue(culture, segment, published) is string value && long.TryParse(value, CultureInfo.InvariantCulture, out var number)
            ? [new IndexField(property.Alias, new IndexValue { Decimals = [number] }, culture, segment)]
            : [];
}
