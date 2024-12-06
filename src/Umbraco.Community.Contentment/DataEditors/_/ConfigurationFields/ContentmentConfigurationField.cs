// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

namespace Umbraco.Cms.Core.PropertyEditors;

// TODO: [LK] Review this temporary class to allow the project to build.
public class ContentmentConfigurationField
{
    public string? Key { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    // TODO: [LK] Deprecate this; migrate to use `PropertyEditorUiAlias`.
    [Obsolete("To be removed in Contentment 7.0. Migrate to use `PropertyEditorUiAlias`.")]
    public string? View { get; set; }

    public string? PropertyEditorUiAlias { get; set; }

    public IDictionary<string, object>? Config { get; set; }

    [Obsolete("To be removed in Contentment 7.0")]
    public bool HideLabel { get; set; }
}
