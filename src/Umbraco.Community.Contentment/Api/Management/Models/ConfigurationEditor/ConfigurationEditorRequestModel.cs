// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

namespace Umbraco.Cms.Api.Management.Models.Contentment;

public class ConfigurationEditorItemRequestModel
{
    public string? Key { get; set; }

    public Dictionary<string, object>? Value { get; set; }
}
