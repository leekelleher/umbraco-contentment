/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class DropdownConfigurationEditor : ConfigurationEditor
    {
        public DropdownConfigurationEditor()
            : base()
        {
            var listFields = new[]
            {
                new ConfigurationField
                {
                    Key = "label",
                    Name = "Label",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "value",
                    Name = "Value",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "enabled",
                    Name = "Enabled",
                    View = "boolean",
                    Config = new Dictionary<string, object> { { "default", "1" } }
                },
            };

            Fields.Add(
                Constants.Conventions.ConfigurationEditors.Items,
                "Options",
                "Configure the option items for the dropdown list.",
                IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { "fields", listFields },
                    { Constants.Conventions.ConfigurationEditors.MaxItems, 0 },
                    { Constants.Conventions.ConfigurationEditors.DisableSorting, Constants.Values.False },
                    { "restrictWidth", Constants.Values.True },
                    { "usePrevalueEditors", Constants.Values.False }
                });
            Fields.AddHideLabel();
        }
    }
}
