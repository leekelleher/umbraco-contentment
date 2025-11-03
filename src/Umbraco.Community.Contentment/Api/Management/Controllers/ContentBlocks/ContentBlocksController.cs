// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Api.Management;

[ApiExplorerSettings(GroupName = "Content Blocks")]
[ApiVersion("1.0")]
[ContentmentVersionedApiBackOfficeRoute("content-blocks")]
public class ContentBlocksController : ContentmentControllerBase
{
    private readonly IContentTypeService _contentTypeService;

    public ContentBlocksController(IContentTypeService contentTypeService)
    {
        _contentTypeService = contentTypeService;
    }

    [HttpGet("element-types", Name = "GetElementTypes")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(List<DataListItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetElementTypes()
    {
        // NOTE: A placeholder async task, until I get async working throughout the codebase. ¯\_(ツ)_/¯
        await Task.Run(() => { });

        var elementTypes = _contentTypeService
            .GetAllElementTypes()?
            .OrderBy(x => x.Name ?? x.Alias)
            .Select(x => new DataListItem
            {
                Name = x.Name ?? x.Alias,
                Value = x.Key.ToString(),
                Description = x.Description,
                Icon = x.Icon,
            })
            .ToList();

        return Ok(elementTypes);
    }
}
