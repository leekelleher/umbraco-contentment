// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Umbraco.Cms.Api.Management.Routing;

namespace Umbraco.Community.Contentment.Api.Management;

internal class ContentmentVersionedApiBackOfficeRouteAttribute : VersionedApiBackOfficeRouteAttribute
{
    public ContentmentVersionedApiBackOfficeRouteAttribute(string template)
        : base($"{Constants.Internals.ProjectAlias}/{template.TrimStart('/')}")
    { }
}
