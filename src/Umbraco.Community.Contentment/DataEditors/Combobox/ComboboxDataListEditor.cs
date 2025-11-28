// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors;

public sealed class ComboboxDataListEditor : IContentmentListEditor
{
    internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "Combobox";

    public string Name => "Combobox";

    public string Description => "Select a single value from a combobox list.";

    public string Icon => "icon-box-alt";

    public string? Group => default;

    public IEnumerable<ContentmentConfigurationField> Fields =>
    [
        new ShowDescriptionsConfigurationField(),
        new ShowIconsConfigurationField(),
    ];

    public Dictionary<string, object> DefaultValues => new()
    {
        { ShowDescriptionsConfigurationField.ShowDescriptions, true },
        { ShowIconsConfigurationField.ShowIcons, true },
    };

    public Dictionary<string, object>? DefaultConfig => default;

    public bool HasMultipleValues(Dictionary<string, object>? config) => false;

    public OverlaySize OverlaySize => OverlaySize.Small;

    public string PropertyEditorUiAlias => DataEditorUiAlias;
}
