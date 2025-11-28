/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class ButtonsDataListEditor : IContentmentListEditor
    {
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "buttons.html";

        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "Buttons";

        public string Name => "Buttons";

        public string Description => "Select multiple values from a group of buttons.";

        public string Icon => "icon-tab";

        public string? Group => default;

        public IEnumerable<ContentmentConfigurationField> Fields =>
        [
            new ContentmentConfigurationField
            {
                Key = "defaultIcon",
                Name = "Default icon",
                Description = "Select an icon to be displayed as the default icon,<br><em>(for when no icon is available)</em>.",
                PropertyEditorUiAlias = IconPickerDataEditor.DataEditorUiAlias,
            },
            new ContentmentConfigurationField
            {
                Key = "size",
                Name = "Size",
                Description = "Select the button size. By default this is set to 'medium'.",
                PropertyEditorUiAlias = RadioButtonListDataListEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "Small", Value = "s" },
                            new DataListItem { Name = "Medium", Value = "m" },
                            new DataListItem { Name = "Large", Value = "l" },
                        }
                    },
                }
            },
            new ContentmentConfigurationField
            {
                Key = "labelStyle",
                Name = "Label style",
                Description = "Select the style of the button's label.",
                PropertyEditorUiAlias = RadioButtonListDataListEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "Icon and Text", Value = "both", Description = "Displays both the item's icon and name." },
                            new DataListItem { Name = "Icon only", Value = "icon", Description = "Hides the item's name and only displays the icon." },
                            new DataListItem { Name = "Text only", Value = "text", Description = "Hides the item's icon and only displays the name." },
                        }
                    },
                    { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, "both" },
                    { ShowDescriptionsConfigurationField.ShowDescriptions, true },
                }
            },
            new ContentmentConfigurationField
            {
                Key = "look",
                Name = "Button style",
                Description = "Select the style of the button. The fallback style is 'secondary'.",
                PropertyEditorUiAlias = RadioButtonListDataListEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "Default", Value = "default" },
                            new DataListItem { Name = "Outline", Value = "outline" },
                            new DataListItem { Name = "Placeholder", Value = "placeholder" },
                            new DataListItem { Name = "Primary", Value = "primary" },
                            new DataListItem { Name = "Secondary", Value = "secondary" },
                        }
                    },
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
        ];

        public Dictionary<string, object> DefaultValues => new()
        {
            { "defaultIcon", UmbConstants.Icons.DefaultIcon },
            { "labelStyle", "both" },
            { "look", "secondary" },
            { "size", "m" },
        };

        public Dictionary<string, object>? DefaultConfig => default;

        public bool HasMultipleValues(Dictionary<string, object>? config)
        {
            return config?.TryGetValue("enableMultiple", out var tmp) == true && tmp.TryConvertTo<bool>().Result;
        }

        public OverlaySize OverlaySize => OverlaySize.Small;

        [Obsolete("To be removed in Contentment 8.0. Migrate to use `PropertyEditorUiAlias`.")]
        public string View => DataEditorViewPath;

        public string PropertyEditorUiAlias => DataEditorUiAlias;
    }
}
