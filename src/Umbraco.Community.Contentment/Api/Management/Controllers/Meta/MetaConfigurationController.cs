// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Umbraco.Community.Contentment.Api.Management;

[ApiExplorerSettings(GroupName = "Meta")]
[ApiVersion("1.0")]
[ContentmentVersionedApiBackOfficeRoute("meta")]
public sealed class MetaConfigurationController : ContentmentControllerBase
{
    private readonly IOptions<ContentmentSettings> _settings;

    public MetaConfigurationController(IOptions<ContentmentSettings> settings)
    {
        _settings = settings;
    }

    [HttpGet("configuration", Name = "GetConfiguration")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(MetaConfigurationResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConfiguration()
    {
        var model = new MetaConfigurationResponseModel
        {
            Name = Constants.Internals.ProjectName,
            Version = ContentmentVersion.Version,
            Features = _settings.Value,
        };

        return Ok(model);
    }
}

