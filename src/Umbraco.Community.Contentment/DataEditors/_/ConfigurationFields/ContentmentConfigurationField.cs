// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using System.Text.Json.Serialization;

namespace Umbraco.Cms.Core.PropertyEditors;

public class ContentmentConfigurationField
{
    public string? Key { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? PropertyEditorUiAlias { get; set; }

    [JsonConverter(typeof(ContentmentConfigurationFieldConfigJsonConverter))]
    public IDictionary<string, object>? Config { get; set; }
}
