/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class DataTableConfigurationEditor : ConfigurationEditor
    {
        public DataTableConfigurationEditor()
            : base()
        {
            // TODO: Need to decide how to set the fields, would it be from a DocType, Macro, a POCO, or manually (or offer all options?) [LK:2019-05-15]
            // As a work-in-progress, here is a prototype of the manual approach...

            // NOTE: Excluded these ParameterEditors, as they don't fully support zero-config.
            var exclusions = new[] { "contentpicker", "mediapicker", "entitypicker" };
            var paramEditors = Current.ParameterEditors
                .Select(x => new { label = x.Name, value = x.GetValueEditor().View })
                .Where(x => exclusions.Contains(x.value) == false)
                .OrderBy(x => x.label)
                .ToList();

            var listFields = new[]
            {
                new ConfigurationField
                {
                    Key = "key",
                    Name = "Key",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "label",
                    Name = "Name",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "view",
                    Name = "Editor",
                    View = IOHelper.ResolveUrl(DropdownDataEditor.DataEditorViewPath),
                    Config = new Dictionary<string, object>
                    {
                        { Constants.Conventions.ConfigurationEditors.Items, paramEditors }
                    }
                },
            };

            Fields.Add(
                "fields",
                "Fields",
                "Configure the editor fields for the data table.",
                IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { "fields", listFields },
                    { Constants.Conventions.ConfigurationEditors.MaxItems, "0" },
                    { Constants.Conventions.ConfigurationEditors.DisableSorting, "0" },
                    { "restrictWidth", "1" },
                    { "usePrevalueEditors", "0" }
                });
            Fields.AddMaxItems();
            Fields.Add(
                "restrictWidth",
                "Restrict Width",
                "Select to restrict the width of the data table. This will attempt to make the table to be the same width as the 'Add' button.",
                "boolean");
            Fields.AddHideLabel();
            Fields.AddDisableSorting();
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            config.Add("usePrevalueEditors", "0");

            return config;
        }
    }
}
