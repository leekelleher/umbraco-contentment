// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using System.Collections.Concurrent;
using System.Web;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.ViewModels.DataType;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Api.Management;

[ApiVersion("1.0")]
[ContentmentVersionedApiBackOfficeRoute("data-picker")]
public class DataPickerController : ContentmentDataSourceControllerBase
{
    private readonly IDataTypeService _dataTypeService;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ConfigurationEditorUtility _utility;

    private static readonly ConcurrentDictionary<Guid, (IDataPickerSource, Dictionary<string, object>)> _lookup = [];

    public DataPickerController(
        IAuthorizationService authorizationService,
        IDataTypeService dataTypeService,
        IJsonSerializer jsonSerializer,
        ConfigurationEditorUtility utility)
        : base(authorizationService)
    {
        _dataTypeService = dataTypeService;
        _jsonSerializer = jsonSerializer;
        _utility = utility;
    }

    [HttpPost("editor", Name = "PostDataPickerEditor")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetEditor(DataPickerEditorRequestModel model)
    {
        SetCurrentContentContextValues(model.Id, model.ParentId, model.Variant, model.Alias, model.IsNew, model.ContentTypeKey);

        var propertyEditorUiAlias = string.Empty;

        var config = new Dictionary<string, object>();

        // TODO: [LK] Move all this logic to its own service.

        // NOTE: The configuration is read from the saved Data Type, anything supplied in the request is ignored.
        var resolved = await GetDataSourceAsync(model.DataTypeKey);

        if (resolved is null && model.DataSource?.Key is string key1 && string.IsNullOrWhiteSpace(key1) == false)
        {
            // NOTE: The configuration is supplied in the request, so it is only permitted for those who can edit Data Types.
            // It is never cached, as it hasn't been saved against the Data Type.
            if (await CanSupplyConfigurationAsync() == false)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var source = _utility.GetConfigurationEditor<IDataPickerSource>(key1);
            var sourceConfig = model.DataSource?.Value;
            if (source is not null && sourceConfig is not null)
            {
                resolved = (source, sourceConfig);
            }
        }

        if (resolved is not null)
        {
            var items = await resolved.Value.Item1.GetItemsAsync(resolved.Value.Item2, model.Values ?? []) ?? [];
            config.Add(nameof(items), items);
        }

        var key2 = model.DisplayMode?.Key;
        if (string.IsNullOrWhiteSpace(key2) == false)
        {
            var editor = _utility.GetConfigurationEditor<IContentmentDisplayMode>(key2);

            propertyEditorUiAlias = editor?.PropertyEditorUiAlias ?? key2;

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

    [Obsolete("Please call the `Search` endpoint using a POST method. This method will be removed in Contentment 8.0.")]
    [HttpGet("search", Name = "GetDataPickerSearch")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Search(
        string id,
        Guid dataTypeKey,
        int pageNumber = 1,
        int pageSize = 12,
        string query = "",
        string? alias = default,
        string? variant = default)
            => await Search(id, dataTypeKey, pageNumber, pageSize, query, alias, variant, null, false, null, []);

    [HttpPost("search", Name = "PostDataPickerSearch")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Search(
        string id,
        Guid dataTypeKey,
        int pageNumber = 1,
        int pageSize = 12,
        string query = "",
        string? alias = default,
        string? variant = default,
        string? parentId = default,
        bool? isNew = false,
        Guid? contentTypeKey = default,
        [FromBody] string[]? values = null)
    {
        SetCurrentContentContextValues(id, parentId, variant, alias, isNew, contentTypeKey);

        if (await GetDataSourceAsync(dataTypeKey) is (IDataPickerSource source, Dictionary<string, object> config))
        {
            var results = source is IDataPickerSource2 source2
                ? await source2.SearchAsync(config, pageNumber, pageSize, HttpUtility.UrlDecode(query), values ?? [])
                : await source.SearchAsync(config, pageNumber, pageSize, HttpUtility.UrlDecode(query));

            return Ok(results);
        }

        return NotFound($"Unable to locate data source for data type: '{dataTypeKey}'");
    }

    // Gets the data source (and its configuration) from the saved Data Type, (cached after the first lookup).
    private async Task<(IDataPickerSource, Dictionary<string, object>)?> GetDataSourceAsync(Guid dataTypeKey)
    {
        if (_lookup.TryGetValue(dataTypeKey, out var cached) == true)
        {
            return cached;
        }

        if (await _dataTypeService.GetAsync(dataTypeKey) is IDataType dataType &&
            dataType.EditorAlias.InvariantEquals(DataPickerDataEditor.DataEditorAlias) == true &&
            GetConfigurationEditorItem(_jsonSerializer, dataType, "dataSource") is { } item &&
            _utility.GetConfigurationEditor<IDataPickerSource>(item.Key ?? string.Empty) is { } source)
        {
            var config = item.Value ?? [];

            _ = _lookup.TryAdd(dataTypeKey, (source, config));

            return (source, config);
        }

        return null;
    }

    // NOTE: The internal cache is cleared from `ContentmentDataTypeNotificationHandler` [LK]
    internal static void ClearCache(Guid dataTypeKey)
    {
        _ = _lookup.TryRemove(dataTypeKey, out _);
    }
}
