// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors;

public sealed class CustomComponentListDataListEditor : IContentmentListEditor
{
    internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "CustomComponentList";

    public string Name => "Custom Component List";

    public string Description => "Select items from a list rendered with custom web component.";

    public string Icon => "icon-shipping-box";

    public string? Group => default;

    public IEnumerable<ContentmentConfigurationField> Fields =>
    [
        new ContentmentConfigurationField
        {
            Key = "component",
            Name = "Component",
            PropertyEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "ManifestPicker",
            Description= "Select a custom web component that implements the `contentmentDataListItemUi` extension-type.",
            Config = new Dictionary<string, object>
            {
                { "extensionType", "contentmentDataListItemUi" },
                { "maxItems", 1 },
            }
        },
        new AllowClearConfigurationField(),
        new ContentmentConfigurationField
        {
            Key = "enableMultiple",
            Name = "Multiple selection?",
            Description = "Select to enable picking multiple items.",
            PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
        },
        new ()
        {
            Key = "orientation",
            Name = "Orientation",
            Description = "Select the orientation of the list. By default this is set to 'vertical' (column).",
            PropertyEditorUiAlias = RadioButtonListDataListEditor.DataEditorUiAlias,
            Config = new Dictionary<string, object>
            {
                { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                    {
                        new DataListItem { Name = "Horizontal", Value = "horizontal" },
                        new DataListItem { Name = "Vertical", Value = "vertical" },
                    }
                },
                { "orientation", "horizontal" },
            }
        },
        new()
        {
            Key = "listStyles",
            Name = "List styles",
            Description = "<em>(optional)</em> Enter CSS rules for the list's container , e.g. <code>&lt;ul&gt;</code> element.",
            PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
        },
        new ()
        {
            Key = "listItemStyles",
            Name = "List item styles",
            Description = "<em>(optional)</em> Enter CSS rules for each list item, e.g. <code>&lt;li&gt;</code> element.",
            PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
        },
    ];

    public Dictionary<string, object>? DefaultConfig => default;

    public Dictionary<string, object>? DefaultValues => new()
    {
        { "orientation", "vertical" },
    };

    public bool HasMultipleValues(Dictionary<string, object>? config)
    {
        return config?.TryGetValueAs("enableMultiple", out bool enableMultiple) == true && enableMultiple == true;
    }

    public OverlaySize OverlaySize => OverlaySize.Medium;

    public string PropertyEditorUiAlias => DataEditorUiAlias;
}
