// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Community.Contentment.DataEditors;

namespace Umbraco.Community.Contentment.Api.Management;

[ApiExplorerSettings(GroupName = "Configuration Editor")]
[ApiVersion("1.0")]
[ContentmentVersionedApiBackOfficeRoute("configuration-editor")]
public class ConfigurationEditorController : ContentmentControllerBase
{
    private readonly ConfigurationEditorUtility _utility;

    public ConfigurationEditorController(ConfigurationEditorUtility utility)
    {
        _utility = utility;
    }

    [HttpGet("models", Name = "GetConfigurationEditorEditorModels")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(ConfigurationEditorModelsResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEditorModels(string type)
    {
        // NOTE: A placeholder async task, until I get async working throughout the codebase. ¯\_(ツ)_/¯
        await Task.Run(() => { });

        // TODO: [LK:2024-12-08] Figure out how to get rid of the hard-coded string/aliases.
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

            case "dataListSource":
            case "dataSource":
            {
                var items = _utility.GetConfigurationEditorModels<IDataListSource>();
                var model = new ConfigurationEditorModelsResponseModel { Items = items };
                return Ok(model);
            }

            case "dataListEditor":
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
