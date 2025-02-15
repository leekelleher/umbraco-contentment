// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using System.Text.Json.Nodes;
using System.Web;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.ViewModels.DataType;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Api.Management;

[ApiExplorerSettings(GroupName = "Data Picker")]
[ApiVersion("1.0")]
[ContentmentVersionedApiBackOfficeRoute("data-picker")]
public class DataPickerController : ContentmentControllerBase
{
    private readonly IDataTypeService _dataTypeService;
    private readonly ConfigurationEditorUtility _utility;

    private static readonly Dictionary<Guid, (IDataPickerSource, Dictionary<string, object>)> _lookup = [];

    public DataPickerController(
        IDataTypeService dataTypeService,
        ConfigurationEditorUtility utility)
    {
        _dataTypeService = dataTypeService;
        _utility = utility;
    }

    [HttpPost("editor", Name = "PostDataPickerEditor")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DataPickerEditorResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEditor(DataPickerEditorRequestModel model)
    {
        var propertyEditorUiAlias = string.Empty;

        var config = new Dictionary<string, object>();

        // TODO: [LK] Move all this logic to its own service.

        if (_lookup.TryGetValue(model.DataTypeKey, out var cached) == true)
        {
            var items = await cached.Item1.GetItemsAsync(cached.Item2, model.Values ?? []) ?? [];
            config.Add(nameof(items), items);
        }
        else if (model.DataSource?.Key is string key1 && string.IsNullOrWhiteSpace(key1) == false)
        {
            var source = _utility.GetConfigurationEditor<IDataPickerSource>(key1);
            var sourceConfig = model.DataSource?.Value;
            if (source is not null && sourceConfig is not null)
            {
                _ = _lookup.TryAdd(model.DataTypeKey, (source, sourceConfig));

                var items = await source.GetItemsAsync(sourceConfig, model.Values ?? []) ?? [];
                config.Add(nameof(items), items);
            }
        }

        var key2 = model.DisplayMode?.Key;
        if (string.IsNullOrWhiteSpace(key2) == false)
        {
            var editor = _utility.GetConfigurationEditor<IContentmentDisplayMode>(key2);

            propertyEditorUiAlias = editor?.PropertyEditorUiAlias;

            var editorConfig = model.DisplayMode?.Value;
            if (editorConfig is not null)
            {
                foreach (var prop in editorConfig)
                {
                    _ = config.TryAdd(prop.Key, prop.Value);
                }
            }

            if (editor?.DefaultConfig != null)
            {
                foreach (var prop in editor.DefaultConfig)
                {
                    _ = config.TryAdd(prop.Key, prop.Value);
                }
            }
        }

        var result = new DataListEditorResponseModel
        {
            PropertyEditorUiAlias = propertyEditorUiAlias,
            Config = config.Select(x => new DataTypePropertyPresentationModel { Alias = x.Key, Value = x.Value })
        };

        return Ok(result);
    }

    [HttpGet("search", Name = "GetDataPickerSearch")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(PagedModel<DataListItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Search(
        string id,
        Guid dataTypeKey,
        int pageNumber = 1,
        int pageSize = 12,
        string query = "",
        string? alias = default,
        string? variant = default)
    {
        if (_lookup.TryGetValue(dataTypeKey, out var cached) == true)
        {
            var results = await cached.Item1.SearchAsync(cached.Item2, pageNumber, pageSize, HttpUtility.UrlDecode(query));
            return Ok(results);
        }

        if (await _dataTypeService.GetAsync(dataTypeKey) is IDataType dataType &&
            dataType?.EditorAlias.InvariantEquals(DataPickerDataEditor.DataEditorAlias) == true &&
            dataType.ConfigurationData is Dictionary<string, object> dataTypeConfig &&
            dataTypeConfig.TryGetValue("dataSource", out var tmp1) == true &&
            tmp1 is JsonArray array1 &&
            array1.Count > 0 &&
            array1[0] is JsonObject item1)
        {
            var source1 = _utility.GetConfigurationEditor<IDataPickerSource>(item1.GetValueAsString("key") ?? string.Empty);
            if (source1 is not null)
            {
                var config1 = item1?["value"]?.ToDictionary<object>() as Dictionary<string, object> ?? [];

                _ = _lookup.TryAdd(dataTypeKey, (source1, config1));

                var results = await source1.SearchAsync(config1, pageNumber, pageSize, HttpUtility.UrlDecode(query));

                return Ok(results);
            }
        }

        return NotFound($"Unable to locate data source for data type: '{dataTypeKey}'");
    }

    // NOTE: The internal cache is cleared from `ContentmentDataTypeNotificationHandler` [LK]
    internal static void ClearCache(Guid dataTypeKey)
    {
        _ = _lookup.Remove(dataTypeKey);
    }
}
