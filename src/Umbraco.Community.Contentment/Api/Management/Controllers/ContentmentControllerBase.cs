// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Web.Common.Authorization;

namespace Umbraco.Community.Contentment.Api.Management;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
[ContentmentVersionedApiBackOfficeRoute("/")]
[JsonOptionsName(UmbConstants.JsonOptionsNames.BackOffice)]
public abstract class ContentmentControllerBase : ManagementApiControllerBase
{
    // Set the current content values, so that `IContentmentContentContext` can access them.
    [Obsolete("Please inherit from `ContentmentDataSourceControllerBase` instead. This method will be removed in Contentment 8.0.")]
    protected void SetCurrentContentContextValues(
        string? contentId = null,
        string? parentId = null,
        string? variantId = null,
        string? propertyAlias = null,
        bool? isNew = false,
        Guid? contentTypeKey = null)
        => ContentmentDataSourceControllerBase.SetHttpContextItems(HttpContext, contentId, parentId, variantId, propertyAlias, isNew, contentTypeKey);
}
