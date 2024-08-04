// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.Models.Contentment;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Api.Management.ViewModels.DataType;
using Umbraco.Community.Contentment;
using Umbraco.Community.Contentment.DataEditors;

namespace Umbraco.Cms.Api.Management.Controllers.Contentment;

[ApiVersion("1.0")]
[VersionedApiBackOfficeRoute($"{Constants.Internals.ProjectAlias}/data-list")]
public class DataListController : ContentmentControllerBase
{
    private readonly ConfigurationEditorUtility _utility;

    public DataListController(ConfigurationEditorUtility utility)
    {
        _utility = utility;
    }

    [HttpPost("editor")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DataListEditorResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEditor(DataListConfigurationRequestModel model)
    {
        // NOTE: A placeholder async task, until I get async working throughout the codebase. ¯\_(ツ)_/¯
        await Task.Run(() => { });

        var propertyEditorUiAlias = string.Empty;

        var config = new Dictionary<string, object>();

        // TODO: [LK] Move all this logic to its own service.

        var key1 = model.DataSource?.FirstOrDefault()?.Key;
        if (string.IsNullOrWhiteSpace(key1) == false)
        {
            var source = _utility.GetConfigurationEditor<IDataListSource>(key1);
            var sourceConfig = model.DataSource?.FirstOrDefault()?.Value;
            if (sourceConfig is not null)
            {
                var items = source?.GetItems(sourceConfig) ?? [];
                config.Add(nameof(items), items);
            }
        }

        var key2 = model.ListEditor?.FirstOrDefault()?.Key;
        if (string.IsNullOrWhiteSpace(key2) == false)
        {
            var editor = _utility.GetConfigurationEditor<IDataListEditor>(key2);

            propertyEditorUiAlias = editor?.PropertyEditorUiAlias;

            var editorConfig = model.ListEditor?.FirstOrDefault()?.Value;
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
}
