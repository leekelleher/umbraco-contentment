// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Search.Indexing;
using Umbraco.Cms.Core.Search.Indexing.Collection;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors;

/// <summary>
/// Indexes the values of the nested properties of every Content Block, each by the handler of its own property editor.
/// </summary>
/// <remarks>
/// All nested values are combined into one index field, under the Content Blocks property's own culture and segment.
/// </remarks>
public sealed class ContentBlocksPropertyValueHandler : IPropertyValueHandler
{
    private readonly IContentTypeService _contentTypeService;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly PropertyValueHandlerCollection _propertyValueHandlers;

    public ContentBlocksPropertyValueHandler(
        IContentTypeService contentTypeService,
        IJsonSerializer jsonSerializer,
        PropertyValueHandlerCollection propertyValueHandlers)
    {
        _contentTypeService = contentTypeService;
        _jsonSerializer = jsonSerializer;
        _propertyValueHandlers = propertyValueHandlers;
    }

    public bool CanHandle(IPropertyType propertyType)
        => propertyType.PropertyEditorAlias == ContentBlocksDataEditor.DataEditorAlias;

    public IEnumerable<IndexField> GetIndexFields(IProperty property, string? culture, string? segment, bool published, IContentBase contentContext)
    {
        var blocks = property.GetJsonValue<IEnumerable<ContentBlock>>(culture, segment, published, _jsonSerializer)?.WhereNotNull().ToArray();
        if (blocks is null || blocks.Length == 0)
        {
            return [];
        }

        var elementTypes = _contentTypeService
            .GetMany(blocks.Select(x => x.ElementType).Distinct())
            .ToDictionary(x => x.Key);

        var nestedPropertyTypes = new Dictionary<Guid, IPropertyType>();
        var indexValues = new List<IndexValue>();

        foreach (var block in blocks)
        {
            if (elementTypes.TryGetValue(block.ElementType, out var elementType) is false)
            {
                continue;
            }

            foreach (var propertyType in elementType.CompositionPropertyTypes)
            {
                // NOTE: A property type may have been removed from the element type since the block was saved.
                if (block.Value.TryGetValue(propertyType.Alias, out var blockPropertyValue) is false)
                {
                    continue;
                }

                if (nestedPropertyTypes.TryGetValue(propertyType.Key, out var nestedPropertyType) is false)
                {
                    // NOTE: The nested values have no culture, segment or published state of their own, so the copy is made invariant.
                    nestedPropertyType = (IPropertyType)propertyType.DeepClone();
                    nestedPropertyType.Variations = ContentVariation.Nothing;
                    nestedPropertyType.SupportsPublishing = false;

                    nestedPropertyTypes.Add(propertyType.Key, nestedPropertyType);
                }

                indexValues.AddRange(NestedPropertyIndexer.GetIndexValues(nestedPropertyType, blockPropertyValue, published, contentContext, _propertyValueHandlers));
            }
        }

        var indexValue = indexValues.Merge();

        return indexValue is not null
            ? [new IndexField(property.Alias, indexValue, culture, segment)]
            : [];
    }
}
