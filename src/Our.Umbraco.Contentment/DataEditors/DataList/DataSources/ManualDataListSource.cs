/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using UmbracoIcons = Umbraco.Core.Constants.Icons;

namespace Our.Umbraco.Contentment.DataEditors
{
#if !DEBUG
    // TODO: Rethink if this data source is necessary. [LK]
    [global::Umbraco.Core.Composing.HideFromTypeFinder]
#endif
    internal class ManualDataListSource : IDataListSource
    {
        public string Name => "Manual";

        public string Description => "Manually configure the items for the data source.";

        public string Icon => UmbracoIcons.DataType;

        [ConfigurationField(typeof(NotesConfigurationField))]
        public string Notes { get; set; }

        [ConfigurationField(typeof(ItemsConfigurationField))]
        public IEnumerable<DataListItem> Items { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            return Items;
        }

        class NotesConfigurationField : ConfigurationField
        {
            public NotesConfigurationField()
            {
                var html = @"<p class='alert alert-warning'><strong>A note manually configuring the data list.</strong><br>
If you intend to use this Data List exclusively with a checkbox, dropdown or radiobutton list type, then you may want to consider not using a Data List editor.<br>
Data List has an overhead <i>(albeit a small overhead)</i> when processing the data source. From a performance perspective it would be better to use a specific Checkbox List, Dropdown List or Radiobutton List editor.</p>";

                Key = NotesConfigurationEditor.Notes;
                Name = nameof(NotesConfigurationEditor.Notes);
                View = IOHelper.ResolveUrl(NotesDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    { NotesConfigurationEditor.Notes, html }
                };
                HideLabel = true;
            }
        }

        class ItemsConfigurationField : ConfigurationField
        {
            public ItemsConfigurationField()
                : base()
            {
                var listFields = new[]
                {
                    new ConfigurationField
                    {
                        Key = "name",
                        Name = "Name",
                        View = "textstring"
                    },
                    new ConfigurationField
                    {
                        Key = "value",
                        Name = "Value",
                        View = "textstring"
                    }
                };

                Key = Constants.Conventions.ConfigurationEditors.Items;
                Name = "Options";
                Description = "Configure the option items for the dropdown list.<br><br>If you use duplicate values, then only the first option item will be used.";
                View = IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>()
                {
                    { DataTableConfigurationEditor.FieldItems, listFields },
                    { DataTableConfigurationEditor.MaxItems, 0 },
                    { DataTableConfigurationEditor.DisableSorting, Constants.Values.False },
                    { DataTableConfigurationEditor.RestrictWidth, Constants.Values.True },
                    { DataTableConfigurationEditor.UsePrevalueEditors, Constants.Values.True }
                };
            }
        }
    }
}
