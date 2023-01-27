/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
#if NET472
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class ItemPickerDataListEditor : IDataListEditor
    {
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "item-picker.html";
        internal const string DataEditorOverlayViewPath = Constants.Internals.EditorsPathRoot + "item-picker.overlay.html";

        private readonly IIOHelper _ioHelper;

        public ItemPickerDataListEditor(IIOHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }

        public string Name => "Item Picker";

        public string Description => "Select items from an Umbraco style overlay.";

        public string Icon => "icon-fa fa-mouse-pointer";

        public string Group => default;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            // TODO: [LK:2022-10-21] Add in "Display Mode" field.

            new ConfigurationField
            {
                Key = "overlaySize",
                Name = "Editor overlay size",
                Description = "Select the size of the overlay editing panel. The default is 'small', although if the editor fields require a wider panel, do consider using 'medium' or 'large'.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(ButtonsDataListEditor.DataEditorViewPath),
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
                    { "labelStyle", "text" },
                    { "size", "m" },
                }
            },
            new ConfigurationField
            {
                Key = "listType",
                Name = "List type",
                Description = "Select the style of list to be displayed in the overlay.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(RadioButtonListDataListEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "Cards", Value = "cards", Description = "Displays as a card based layout." },
                            new DataListItem { Name = "Grid", Value = "grid", Description = "Displays as a grid item based layout." },
                            new DataListItem { Name = "List", Value = "list", Description = "Displays as a single column menu, (with descriptions, if available)." }
                        }
                    },
                    { ShowDescriptionsConfigurationField.ShowDescriptions, Constants.Values.True },
                }
            },
            new ConfigurationField
            {
                Key = "defaultIcon",
                Name = "Default icon",
                Description = "Select an icon to be displayed as the default icon, <em>(for when no icon is available)</em>.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl("~/umbraco/views/propertyeditors/listview/icon.prevalues.html"),
            },
            new EnableFilterConfigurationField
            {
                View = "views/propertyeditors/boolean/boolean.html",
                Config = new Dictionary<string, object>
                {
                    { "default", Constants.Values.True }
                },
            },
            new MaxItemsConfigurationField(_ioHelper),
            new AllowClearConfigurationField(),
            new ConfigurationField
            {
                Key = "allowDuplicates",
                Name = "Allow duplicates?",
                Description = "Select to allow the editor to select duplicate items.",
                View = "boolean",
            },
            new ConfigurationField
            {
                Key = "enableMultiple",
                Name = "Multiple selection?",
                Description = "Select to enable picking multiple items.",
                View = "boolean",
            },
            new DisableSortingConfigurationField(),
            new ConfigurationField
            {
                Key ="confirmRemoval",
                Name = "Confirm removals?",
                Description = "Select to enable a confirmation prompt when removing an item.",
                View = "boolean",
            }
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "listType", "list" },
            { "defaultIcon", UmbConstants.Icons.DefaultIcon },
            { EnableFilterConfigurationField.EnableFilter, Constants.Values.True },
            { MaxItemsConfigurationField.MaxItems, "0" },
        };

        public Dictionary<string, object> DefaultConfig => new Dictionary<string, object>
        {
            { Constants.Conventions.ConfigurationFieldAliases.OverlayView, _ioHelper.ResolveRelativeOrVirtualUrl(DataEditorOverlayViewPath) },
            { "overlayOrderBy", string.Empty },
        };

        public bool HasMultipleValues(Dictionary<string, object> config)
        {
            return config.TryGetValueAs(MaxItemsConfigurationField.MaxItems, out int maxItems) == true && maxItems != 1;
        }

        public OverlaySize OverlaySize => OverlaySize.Medium;

        public string View => DataEditorViewPath;
    }
}
