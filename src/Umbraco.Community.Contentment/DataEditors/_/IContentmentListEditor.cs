// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

namespace Umbraco.Community.Contentment.DataEditors;

public interface IContentmentListEditor : IContentmentEditorItem
{
    Dictionary<string, object>? DefaultConfig { get; }

    bool HasMultipleValues(Dictionary<string, object>? config);

    string PropertyEditorUiAlias { get; }
}
