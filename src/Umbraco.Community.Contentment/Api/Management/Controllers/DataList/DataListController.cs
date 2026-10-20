// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.ViewModels.DataType;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Community.Contentment.DataEditors;

namespace Umbraco.Community.Contentment.Api.Management;

[ApiVersion("1.0")]
[ContentmentVersionedApiBackOfficeRoute("data-list")]
public class DataListController : ContentmentDataSourceControllerBase
{
    private readonly IDataTypeService _dataTypeService;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ConfigurationEditorUtility _utility;

    public DataListController(
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

    [HttpPost("editor", Name = "PostDataListEditor")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetEditor(DataListConfigurationRequestModel model)
    {
        // NOTE: A placeholder async task, until I get async working throughout the codebase. ¯\_(ツ)_/¯
        await Task.Run(() => { });

        SetCurrentContentContextValues(model.Id, model.ParentId, model.Variant, model.Alias, model.IsNew, model.ContentTypeKey);

        var propertyEditorUiAlias = string.Empty;

        var config = new Dictionary<string, object>();

        var (error, dataSourceItem, listEditorItem) = await GetConfigurationAsync(model);
        if (error is not null)
        {
            return error;
        }

        // TODO: [LK] Move all this logic to its own service.

        var key1 = dataSourceItem?.Key;
        if (string.IsNullOrWhiteSpace(key1) == false)
        {
            var source = _utility.GetConfigurationEditor<IContentmentDataSource>(key1);
            var sourceConfig = dataSourceItem?.Value;
            if (source is not null && sourceConfig is not null)
            {
                var items = source.GetItems(sourceConfig) ?? [];
                config.Add(nameof(items), items);
            }
        }

        var key2 = listEditorItem?.Key;
        if (string.IsNullOrWhiteSpace(key2) == false)
        {
            var editor = _utility.GetConfigurationEditor<IContentmentListEditor>(key2);

            propertyEditorUiAlias = editor?.PropertyEditorUiAlias;

            var editorConfig = listEditorItem?.Value;
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

    private async Task<(IActionResult? Error, ConfigurationEditorItemRequestModel? DataSource, ConfigurationEditorItemRequestModel? ListEditor)> GetConfigurationAsync(DataListConfigurationRequestModel model)
    {
        if (model.DataTypeKey.HasValue == false || model.DataTypeKey.Value == Guid.Empty)
        {
            // NOTE: The configuration is supplied in the request, (i.e. the Data Type preview).
            return await CanSupplyConfigurationAsync() == true
                ? (null, model.DataSource, model.ListEditor)
                : (StatusCode(StatusCodes.Status403Forbidden), null, null);
        }

        // NOTE: The configuration is read from the saved Data Type, anything supplied in the request is ignored.
        if (await _dataTypeService.GetAsync(model.DataTypeKey.Value) is not { } dataType)
        {
            return (NotFound($"Unable to locate the data type: '{model.DataTypeKey}'"), null, null);
        }

        return (null, GetConfigurationEditorItem(_jsonSerializer, dataType, "dataSource"), GetConfigurationEditorItem(_jsonSerializer, dataType, "listEditor"));
    }
}
