// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

namespace Umbraco.Community.Contentment.Api.Management;

public class ConfigurationEditorItemRequestModel
{
    public string? Key { get; set; }

    public Dictionary<string, object>? Value { get; set; }
}
