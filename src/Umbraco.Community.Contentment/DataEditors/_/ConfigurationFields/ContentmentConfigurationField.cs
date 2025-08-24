// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

namespace Umbraco.Cms.Core.PropertyEditors;

public class ContentmentConfigurationField
{
    public string? Key { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    [Obsolete("To be removed in Contentment 8.0. Migrate to use `PropertyEditorUiAlias`.")]
    public string? View { get; set; }

    public string? PropertyEditorUiAlias { get; set; }

    public IDictionary<string, object>? Config { get; set; }

    [Obsolete("To be removed in Contentment 8.0")]
    public bool HideLabel { get; set; }
}
