/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class ItemPickerConfigurationEditor : ConfigurationEditor
    {
        public ItemPickerConfigurationEditor()
            : base()
        {
            var listFields = new[]
            {
                 new ConfigurationField
                {
                    Key = "icon",
                    Name = "Icon",
                    View = IOHelper.ResolveUrl(IconPickerDataEditor.DataEditorViewPath)
                },
                new ConfigurationField
                {
                    Key = "name",
                    Name = "Name",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "value",
                    Name = "Value",
                    View = "textbox"
                },
            };

            Fields.Add(
                Constants.Conventions.ConfigurationEditors.Items,
                "Items",
                "Configure the items for the item picker.",
                IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { "fields", listFields },
                    { Constants.Conventions.ConfigurationEditors.MaxItems, 0 },
                    { Constants.Conventions.ConfigurationEditors.DisableSorting, Constants.Values.False },
                    { "restrictWidth", Constants.Values.True },
                    { "usePrevalueEditors", Constants.Values.False }
                });
            Fields.AddMaxItems();
            Fields.Add(
                "allowDuplicates",
                "Allow duplicates?",
                "Select to allow the editor to select duplicate entities.",
                "boolean");
            Fields.AddDisableSorting();
            Fields.AddHideLabel();
        }
    }
}
