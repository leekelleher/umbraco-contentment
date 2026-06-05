// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

using System.Collections.Concurrent;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors;

internal sealed class InputListDataValueEditor : DataValueEditor, IDataValueReference
{
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IDataTypeService _dataTypeService;
    private readonly PropertyEditorCollection _propertyEditors;
    private readonly IDataTypeConfigurationCache _dataTypeConfigurationCache;
    private readonly IShortStringHelper _shortStringHelper;

    private readonly ConcurrentDictionary<Guid, IDataType?> _dataTypeCache = new();

    public InputListDataValueEditor(
        IShortStringHelper shortStringHelper,
        IJsonSerializer jsonSerializer,
        IDataTypeService dataTypeService,
        PropertyEditorCollection propertyEditors,
        IDataTypeConfigurationCache dataTypeConfigurationCache)
    : base(shortStringHelper, jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
        _dataTypeService = dataTypeService;
        _propertyEditors = propertyEditors;
        _dataTypeConfigurationCache = dataTypeConfigurationCache;
        _shortStringHelper = shortStringHelper;

        ValueType = ValueTypes.Json;
    }

    public override object ToEditor(IProperty property, string? culture = null, string? segment = null)
    {
        var value = base.ToEditor(property, culture, segment)?.ToString();
        if (string.IsNullOrWhiteSpace(value) == false)
        {
            var model = _jsonSerializer.Deserialize<InputListValueModel>(value);
            if (model?.Count > 0)
            {
                foreach (var row in model)
                {
                    foreach (var item in row.Values)
                    {
                        var dataType = GetDataType(item.DataType);
                        if (dataType is null)
                        {
                            continue;
                        }

                        if (_propertyEditors.TryGet(dataType.EditorAlias, out var editor) == false)
                        {
                            continue;
                        }

                        var fauxPropertyType = new PropertyType(_shortStringHelper, dataType)
                        {
                            Variations = ContentVariation.Nothing,
                        };
                        var fauxProperty = new Property(fauxPropertyType);
                        fauxProperty.SetValue(item.Value);

                        item.Value = editor.GetValueEditor()?.ToEditor(fauxProperty);
                    }
                }

                return model;
            }
        }

        return Array.Empty<object>();
    }

    public override object? FromEditor(ContentPropertyData editorValue, object? currentValue)
    {
        var value = editorValue.Value?.ToString();
        if (string.IsNullOrWhiteSpace(value) == false)
        {
            var model = _jsonSerializer.Deserialize<InputListValueModel>(value);
            if (model?.Count > 0)
            {
                foreach (var row in model)
                {
                    foreach (var item in row.Values)
                    {
                        var dataType = GetDataType(item.DataType);
                        if (dataType is null)
                        {
                            continue;
                        }

                        if (_propertyEditors.TryGet(dataType.EditorAlias, out var editor) == false)
                        {
                            continue;
                        }

                        var configuration = _dataTypeConfigurationCache.GetConfiguration(item.DataType);
                        var contentPropertyData = new ContentPropertyData(item.Value, configuration)
                        {
                            ContentKey = editorValue.ContentKey,
                        };

                        item.Value = editor.GetValueEditor(configuration)?.FromEditor(contentPropertyData, item.Value);
                    }
                }

                return _jsonSerializer.Serialize(model);
            }
        }

        return base.FromEditor(editorValue, currentValue);
    }

    public IEnumerable<UmbracoEntityReference> GetReferences(object? value)
    {
        if (value is string str && string.IsNullOrWhiteSpace(str) == false && str.DetectIsJson() == true)
        {
            var model = _jsonSerializer.Deserialize<InputListValueModel>(str);
            if (model?.Count > 0)
            {
                foreach (var row in model)
                {
                    foreach (var item in row.Values)
                    {
                        var dataType = GetDataType(item.DataType);
                        if (dataType is null)
                        {
                            continue;
                        }

                        if (_propertyEditors.TryGet(dataType.EditorAlias, out var editor) == false)
                        {
                            continue;
                        }

                        if (editor.GetValueEditor() is IDataValueReference dvr)
                        {
                            foreach (var reference in dvr.GetReferences(item.Value))
                            {
                                yield return reference;
                            }
                        }
                    }
                }
            }
        }
    }

    // IDataValueEditor.ToEditor/FromEditor are synchronous contracts; .GetAwaiter().GetResult() is acceptable here.
    private IDataType? GetDataType(Guid key)
        => _dataTypeCache.GetOrAdd(key, k => _dataTypeService.GetAsync(k).GetAwaiter().GetResult());
}
