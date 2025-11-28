// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

namespace Umbraco.Community.Contentment.DataEditors;

public interface IContentmentListEditor : IContentmentEditorItem
{
    public Dictionary<string, object>? DefaultConfig { get; }

    public bool HasMultipleValues(Dictionary<string, object>? config);

    public string PropertyEditorUiAlias { get; }
}
