/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class ItemPickerDataListEditor : IDataListEditor
    {
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "item-picker.html";

        [Obsolete("To be removed in Contentment 7.0", true)]
        internal const string DataEditorOverlayViewPath = Constants.Internals.EditorsPathRoot + "item-picker.overlay.html";

        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "ItemPicker";

        private readonly IIOHelper _ioHelper;

        public ItemPickerDataListEditor(IIOHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }

        public string Name => "Item Picker";

        public string Description => "Select items from an Umbraco style overlay.";

        public string Icon => "icon-fa-arrow-pointer";

        public string? Group => default;

        public IEnumerable<ContentmentConfigurationField> Fields => new ContentmentConfigurationField[]
        {
            // TODO: [LK:2022-10-21] Add in "Display Mode" field.

            new ContentmentConfigurationField
            {
                Key = "overlaySize",
                Name = "Editor overlay size",
                Description = "Select the size of the overlay editing panel. The default is 'small', although if the editor fields require a wider panel, do consider using 'medium' or 'large'.",
                PropertyEditorUiAlias = RadioButtonListDataListEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "Small", Value = "small" },
                            new DataListItem { Name = "Medium", Value = "medium" },
                            new DataListItem { Name = "Large", Value = "large" }
                        }
                    },
                    { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, "small" },
                }
            },
            new ContentmentConfigurationField
            {
                Key = "listType",
                Name = "List type",
                Description = "Select the style of list to be displayed in the overlay.",
                PropertyEditorUiAlias = RadioButtonListDataListEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "Cards", Value = "cards", Description = "Displays as a card based layout." },
                            new DataListItem { Name = "Grid", Value = "grid", Description = "Displays as a grid item based layout." },
                            new DataListItem { Name = "List", Value = "list", Description = "Displays as a single column menu, (with descriptions, if available)." }
                        }
                    },
                    { ShowDescriptionsConfigurationField.ShowDescriptions, true },
                }
            },
            new ContentmentConfigurationField
            {
                Key = "defaultIcon",
                Name = "Default icon",
                Description = "Select an icon to be displayed as the default icon, <em>(for when no icon is available)</em>.",
                PropertyEditorUiAlias = IconPickerDataEditor.DataEditorUiAlias,
            },
            new EnableFilterConfigurationField
            {
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
                Config = new Dictionary<string, object>
                {
                    { "default", true }
                },
            },
            new MaxItemsConfigurationField(),
            new AllowClearConfigurationField(),
            new ContentmentConfigurationField
            {
                Key = "allowDuplicates",
                Name = "Allow duplicates?",
                Description = "Select to allow the editor to select duplicate items.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
            },
            new ContentmentConfigurationField
            {
                Key = "enableMultiple",
                Name = "Multiple selection?",
                Description = "Select to enable picking multiple items.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
            },
            new DisableSortingConfigurationField(),
            new ContentmentConfigurationField
            {
                Key ="confirmRemoval",
                Name = "Confirm removals?",
                Description = "Select to enable a confirmation prompt when removing an item.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
            }
        };

        public Dictionary<string, object> DefaultValues => new()
        {
            { "listType", "list" },
            { "defaultIcon", UmbConstants.Icons.DefaultIcon },
            { EnableFilterConfigurationField.EnableFilter, true },
            { MaxItemsConfigurationField.MaxItems, "0" },
        };

        public Dictionary<string, object>? DefaultConfig => default;

        public bool HasMultipleValues(Dictionary<string, object>? config)
        {
            return config?.TryGetValueAs(MaxItemsConfigurationField.MaxItems, out int maxItems) == true && maxItems != 1;
        }

        public OverlaySize OverlaySize => OverlaySize.Medium;

        [Obsolete("To be removed in Contentment 7.0. Migrate to use `PropertyEditorUiAlias`.")]
        public string View => DataEditorViewPath;

        public string PropertyEditorUiAlias => DataEditorUiAlias;
    }
}
