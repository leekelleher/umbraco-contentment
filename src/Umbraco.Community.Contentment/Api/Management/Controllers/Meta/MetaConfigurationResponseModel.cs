// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

namespace Umbraco.Community.Contentment.Api.Management;

public sealed class MetaConfigurationResponseModel
{
    public string? Name { get; set; }

    public Version? Version { get; set; }

    public ContentmentSettings? Features { get; set; }
}
