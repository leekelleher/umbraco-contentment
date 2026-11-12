// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Search.Indexing;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors;

/// <summary>
/// Indexes the name of each social link as text and its network as a keyword. The URL is not indexed.
/// </summary>
public sealed class SocialLinksPropertyValueHandler : IPropertyValueHandler
{
    private readonly IJsonSerializer _jsonSerializer;

    public SocialLinksPropertyValueHandler(IJsonSerializer jsonSerializer)
        => _jsonSerializer = jsonSerializer;

    public bool CanHandle(IPropertyType propertyType)
        => propertyType.PropertyEditorAlias == SocialLinksDataEditor.DataEditorAlias;

    public IEnumerable<IndexField> GetIndexFields(IProperty property, string? culture, string? segment, bool published, IContentBase contentContext)
    {
        var links = property.GetJsonValue<IEnumerable<SocialLink>>(culture, segment, published, _jsonSerializer);
        if (links is null)
        {
            return [];
        }

        var texts = new List<string>();
        var keywords = new HashSet<string>();
        foreach (var link in links)
        {
            if (string.IsNullOrWhiteSpace(link.Name) is false)
            {
                texts.Add(link.Name);
            }

            if (string.IsNullOrWhiteSpace(link.Network) is false)
            {
                keywords.Add(link.Network);
            }
        }

        if (texts.Count == 0 && keywords.Count == 0)
        {
            return [];
        }

        return [new IndexField(property.Alias, new IndexValue { Texts = texts, Keywords = keywords }, culture, segment)];
    }
}
