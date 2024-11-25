// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Web.Common.Authorization;

namespace Umbraco.Community.Contentment.Api.Management;

[ApiExplorerSettings(GroupName = Constants.Internals.ProjectName)]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
[ContentmentVersionedApiBackOfficeRoute("/")]
[JsonOptionsName(UmbConstants.JsonOptionsNames.BackOffice)]
[MapToApi(Constants.Internals.ProjectAlias)]
public abstract class ContentmentControllerBase : ManagementApiControllerBase { }
