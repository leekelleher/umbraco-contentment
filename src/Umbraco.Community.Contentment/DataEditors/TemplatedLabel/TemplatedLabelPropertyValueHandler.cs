// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using System.Globalization;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Search;
using Umbraco.Cms.Core.Search.Indexing;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors;

/// <summary>
/// Indexes a Templated Label by its configured data value type, mirroring Umbraco's own label handler.
/// </summary>
public sealed class TemplatedLabelPropertyValueHandler : IPropertyValueHandler
{
    private readonly IDataTypeConfigurationCache _dataTypeConfigurationCache;
    private readonly IDateTimeOffsetConverter _dateTimeOffsetConverter;

    public TemplatedLabelPropertyValueHandler(
        IDataTypeConfigurationCache dataTypeConfigurationCache,
        IDateTimeOffsetConverter dateTimeOffsetConverter)
    {
        _dataTypeConfigurationCache = dataTypeConfigurationCache;
        _dateTimeOffsetConverter = dateTimeOffsetConverter;
    }

    public bool CanHandle(IPropertyType propertyType)
        => propertyType.PropertyEditorAlias == TemplatedLabelDataEditor.DataEditorAlias;

    public IEnumerable<IndexField> GetIndexFields(IProperty property, string? culture, string? segment, bool published, IContentBase contentContext)
    {
        var value = property.GetValue(culture, segment, published);
        if (value is null || string.Empty.Equals(value))
        {
            return [];
        }

        var valueType = ValueTypes.String;
        if (_dataTypeConfigurationCache.GetConfiguration(property.PropertyType.DataTypeKey) is Dictionary<string, object> config &&
            config.TryGetValueAs(UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType, out string? configuredValueType) &&
            string.IsNullOrWhiteSpace(configuredValueType) is false &&
            ValueTypes.IsValue(configuredValueType))
        {
            valueType = configuredValueType;
        }

        var indexValue = valueType switch
        {
            ValueTypes.Integer when value is int integer => new IndexValue { Integers = [integer] },
            ValueTypes.Decimal when value is decimal number => new IndexValue { Decimals = [number] },
            ValueTypes.Bigint when value is string bigint && long.TryParse(bigint, CultureInfo.InvariantCulture, out var number) => new IndexValue { Decimals = [number] },
            ValueTypes.Date or ValueTypes.DateTime when value is DateTime date => new IndexValue { DateTimeOffsets = [_dateTimeOffsetConverter.ToDateTimeOffset(date)] },
            ValueTypes.String or ValueTypes.Text when value is string text => new IndexValue { Texts = [text] },
            _ => null,
        };

        return indexValue is not null
            ? [new IndexField(property.Alias, indexValue, culture, segment)]
            : [];
    }
}
