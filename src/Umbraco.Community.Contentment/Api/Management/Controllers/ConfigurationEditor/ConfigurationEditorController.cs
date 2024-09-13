// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.Models.Contentment;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Community.Contentment;
using Umbraco.Community.Contentment.DataEditors;

namespace Umbraco.Cms.Api.Management.Controllers.Contentment;

[ApiVersion("1.0")]
[VersionedApiBackOfficeRoute($"{Constants.Internals.ProjectAlias}/configuration-editor")]
public class ConfigurationEditorController : ContentmentControllerBase
{
    private readonly ConfigurationEditorUtility _utility;

    public ConfigurationEditorController(ConfigurationEditorUtility utility)
    {
        _utility = utility;
    }

    [HttpGet("models")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(ConfigurationEditorModelsResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEditorModels(string type)
    {
        // NOTE: A placeholder async task, until I get async working throughout the codebase. ¯\_(ツ)_/¯
        await Task.Run(() => { });

        switch (type)
        {
            case "dataPickerSource":
            {
                var items = _utility.GetConfigurationEditorModels<IDataPickerSource>();
                var model = new ConfigurationEditorModelsResponseModel { Items = items };
                return Ok(model);
            }

            case "dataPickerDisplayMode":
            {
                var items = _utility.GetConfigurationEditorModels<IDataPickerDisplayMode>();
                var model = new ConfigurationEditorModelsResponseModel { Items = items };
                return Ok(model);
            }

            case "dataSource":
            {
                var items = _utility.GetConfigurationEditorModels<IDataListSource>();
                var model = new ConfigurationEditorModelsResponseModel { Items = items };
                return Ok(model);
            }

            case "listEditor":
            {
                var items = _utility.GetConfigurationEditorModels<IDataListEditor>();
                var model = new ConfigurationEditorModelsResponseModel { Items = items };
                return Ok(model);
            }

            default:
                break;
        }

        return NotFound();
    }
}
