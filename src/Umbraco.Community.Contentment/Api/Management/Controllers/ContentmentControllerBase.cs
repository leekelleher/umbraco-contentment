// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Community.Contentment;

namespace Umbraco.Cms.Api.Management.Controllers.Contentment;

[ApiExplorerSettings(GroupName = Constants.Internals.ProjectName)]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
[VersionedApiBackOfficeRoute(Constants.Internals.ProjectAlias)]
public abstract class ContentmentControllerBase : ManagementApiControllerBase { }
