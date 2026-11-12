// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using System.Text.Json.Nodes;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Search.Indexing;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors;

/// <summary>
/// Indexes the editors that store values from a configured data source (Data List and Data Picker), through that data source, see <see cref="IDataSourceValueIndexer"/>.
/// Without one, each stored value is indexed as a keyword.
/// </summary>
public sealed class DataSourcePropertyValueHandler : IPropertyValueHandler
{
    private readonly IDataTypeConfigurationCache _dataTypeConfigurationCache;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ConfigurationEditorUtility _utility;

    public DataSourcePropertyValueHandler(
        IDataTypeConfigurationCache dataTypeConfigurationCache,
        IJsonSerializer jsonSerializer,
        ConfigurationEditorUtility utility)
    {
        _dataTypeConfigurationCache = dataTypeConfigurationCache;
        _jsonSerializer = jsonSerializer;
        _utility = utility;
    }

    public bool CanHandle(IPropertyType propertyType)
        => propertyType.PropertyEditorAlias is
            DataListDataEditor.DataEditorAlias
            or DataPickerDataEditor.DataEditorAlias;

    public IEnumerable<IndexField> GetIndexFields(IProperty property, string? culture, string? segment, bool published, IContentBase contentContext)
    {
        if (property.GetValue(culture, segment, published) is not string value || string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        // NOTE: A value saved as single-value may not be JSON, see `DataListValueConverter`.
        var items = value.DetectIsJson() is true
            ? _jsonSerializer.Deserialize<IEnumerable<string>>(value)
            : [value];

        var keys = items?.Where(x => string.IsNullOrWhiteSpace(x) is false).ToArray();
        if (keys is null || keys.Length == 0)
        {
            return [];
        }

        IDataSourceValueIndexer? indexer = null;
        Dictionary<string, object>? config = null;

        // NOTE: Reads the data source the same way as the value converters do, e.g. `DataListValueConverter`.
        if (_dataTypeConfigurationCache.GetConfiguration(property.PropertyType.DataTypeKey) is Dictionary<string, object> configuration &&
            configuration.TryGetValue("dataSource", out var tmp) is true &&
            tmp is JsonArray array && array.Count > 0 && array[0] is JsonObject obj &&
            obj.GetValueAsString("key") is string key)
        {
            indexer = _utility.GetConfigurationEditor<IDataSourceValueIndexer>(key);
            if (indexer is not null)
            {
                config = _jsonSerializer.Deserialize<Dictionary<string, object>>(obj["value"]?.ToString() ?? "{}");
            }
        }

        var indexValue = indexer is not null
            ? keys.Select(x => indexer.GetIndexValue(config, x)).WhereNotNull().Merge()
            : new IndexValue { Keywords = keys };

        return indexValue is not null
            ? [new IndexField(property.Alias, indexValue, culture, segment)]
            : [];
    }
}
