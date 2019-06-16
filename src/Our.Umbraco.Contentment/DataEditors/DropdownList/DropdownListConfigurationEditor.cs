/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class DropdownListConfigurationEditor : ConfigurationEditor
    {
        public const string AllowEmpty = "allowEmpty";
        public const string Items = "items";
        // TODO: [LK:2019-06-16] Implement "DefaultValue"
        public const string DefaultValue = "defaultValue";

        public DropdownListConfigurationEditor()
            : base()
        {
            var listFields = new[]
            {
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
                //new ConfigurationField
                //{
                //    Key = "enabled",
                //    Name = "Enabled",
                //    View = "boolean",
                //    Config = new Dictionary<string, object> { { "default", Constants.Values.True } }
                //},
            };

            Fields.Add(
                Items,
                "Options",
                "Configure the option items for the dropdown list.",
                IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { DataTableConfigurationEditor.FieldItems, listFields },
                    { MaxItemsConfigurationField.MaxItems, 0 },
                    { DisableSortingConfigurationField.DisableSorting, Constants.Values.False },
                    { DataTableConfigurationEditor.RestrictWidth, Constants.Values.True },
                    { DataTableConfigurationEditor.UsePrevalueEditors, Constants.Values.False }
                });

            Fields.Add(new AllowEmptyConfigurationField());

            Fields.AddHideLabel();
        }

        internal class AllowEmptyConfigurationField : ConfigurationField
        {
            public AllowEmptyConfigurationField()
                : base()
            {
                Key = AllowEmpty;
                Name = "Allow Empty";
                Description = "Enable to allow an empty option at the top of the dropdown list.";
                View = "views/propertyeditors/boolean/boolean.html";
                Config = new Dictionary<string, object>
                {
                    { "default", Constants.Values.True }
                };
            }
        }
    }
}
