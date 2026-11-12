// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Search.Indexing;
using Umbraco.Cms.Core.Search.Indexing.Collection;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors;

/// <summary>
/// Indexes the value of every item in an Input List, each by the handler of the property editor of its data type.
/// </summary>
/// <remarks>
/// All nested values are combined into one index field, under the Input List property's own culture and segment.
/// </remarks>
public sealed class InputListPropertyValueHandler : IPropertyValueHandler
{
    private readonly IDataTypeService _dataTypeService;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly PropertyValueHandlerCollection _propertyValueHandlers;
    private readonly IShortStringHelper _shortStringHelper;

    public InputListPropertyValueHandler(
        IDataTypeService dataTypeService,
        IJsonSerializer jsonSerializer,
        PropertyValueHandlerCollection propertyValueHandlers,
        IShortStringHelper shortStringHelper)
    {
        _dataTypeService = dataTypeService;
        _jsonSerializer = jsonSerializer;
        _propertyValueHandlers = propertyValueHandlers;
        _shortStringHelper = shortStringHelper;
    }

    public bool CanHandle(IPropertyType propertyType)
        => propertyType.PropertyEditorAlias == InputListDataEditor.DataEditorAlias;

    public IEnumerable<IndexField> GetIndexFields(IProperty property, string? culture, string? segment, bool published, IContentBase contentContext)
    {
        var model = property.GetJsonValue<InputListValueModel>(culture, segment, published, _jsonSerializer);
        if (model is null || model.Count == 0)
        {
            return [];
        }

        var nestedPropertyTypes = new Dictionary<Guid, IPropertyType?>();
        var indexValues = new List<IndexValue>();

        foreach (var item in model.SelectMany(x => x.Values))
        {
            if (nestedPropertyTypes.TryGetValue(item.DataType, out var nestedPropertyType) is false)
            {
                // NOTE: Indexing is a synchronous contract, so `.GetAwaiter().GetResult()` is acceptable here, as it is in `InputListDataValueEditor`.
                var dataType = _dataTypeService.GetAsync(item.DataType).GetAwaiter().GetResult();
                nestedPropertyType = dataType is not null
                    ? new PropertyType(_shortStringHelper, dataType)
                    {
                        // NOTE: The nested values have no culture, segment or published state of their own.
                        Variations = ContentVariation.Nothing,
                        SupportsPublishing = false,
                    }
                    : null;

                nestedPropertyTypes.Add(item.DataType, nestedPropertyType);
            }

            if (nestedPropertyType is null)
            {
                continue;
            }

            indexValues.AddRange(NestedPropertyIndexer.GetIndexValues(nestedPropertyType, item.Value, published, contentContext, _propertyValueHandlers));
        }

        var indexValue = indexValues.Merge();

        return indexValue is not null
            ? [new IndexField(property.Alias, indexValue, culture, segment)]
            : [];
    }
}
