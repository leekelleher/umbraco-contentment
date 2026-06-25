// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Api.Management;

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
    public async Task<IActionResult> GetConfiguration()
    {
        var model = new MetaConfigurationResponseModel
        {
            Name = Constants.Internals.ProjectName,
            Version = ContentmentVersion.SemanticVersion?.ToSemanticStringWithoutBuild() ?? "0.0.0",
            Features = _settings.Value,
        };

        return Ok(model);
    }
}

