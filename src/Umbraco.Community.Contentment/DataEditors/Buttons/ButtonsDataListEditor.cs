/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class ButtonsDataListEditor : IDataListEditor
    {
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "buttons.html";

        public string Name => "Buttons";

        public string Description => "Select multiple values from a group of buttons.";

        public string Icon => "icon-tab";

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ConfigurationField
            {
                Key = "defaultIcon",
                Name = "Default icon",
                Description = "Select an icon to be displayed as the default icon, (for when no icon is available).",
                View = IOHelper.ResolveUrl("~/umbraco/views/propertyeditors/listview/icon.prevalues.html"),
            },
            new ConfigurationField
            {
                Key = "size",
                Name = "Size",
                Description = "Select the button size. By default this is set to 'medium'.",
                View = IOHelper.ResolveUrl(RadioButtonListDataListEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "Small", Value = "s" },
                            new DataListItem { Name = "Medium", Value = "m" },
                            new DataListItem { Name = "Large", Value = "l" },
                        }
                    },
                    { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, "m" }
                }
            },
            new ConfigurationField
            {
                Key = "labelStyle",
                Name = "Label style",
                Description = "Select the style of the button's label.",
                View = IOHelper.ResolveUrl(RadioButtonListDataListEditor.DataEditorViewPath),
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
                    { ShowDescriptionsConfigurationField.ShowDescriptions, Constants.Values.True },
                }
            },
            new AllowClearConfigurationField(),
            new ConfigurationField
            {
                Key = "enableMultiple",
                Name = "Multiple selection?",
                Description = "Select to enable picking multiple items.",
                View = "boolean",
            },
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "defaultIcon", Core.Constants.Icons.DefaultIcon },
            { "labelStyle", "both" },
        };

        public Dictionary<string, object> DefaultConfig => default;

        public bool HasMultipleValues(Dictionary<string, object> config)
        {
            return config.TryGetValue("enableMultiple", out var tmp) && tmp.TryConvertTo<bool>().Result;
        }

        public OverlaySize OverlaySize => OverlaySize.Small;

        public string View => DataEditorViewPath;
    }
}
