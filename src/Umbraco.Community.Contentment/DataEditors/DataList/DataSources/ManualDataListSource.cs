/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using UmbracoIcons = Umbraco.Core.Constants.Icons;

namespace Umbraco.Community.Contentment.DataEditors
{
#if !DEBUG
    // TODO: Rethink if this data source is necessary. [LK]
    [global::Umbraco.Core.Composing.HideFromTypeFinder]
#endif
    internal class ManualDataListSource : IDataListSource
    {
        public string Name => "User-defined list";

        public string Description => "Manually configure the items for the data source.";

        public string Icon => UmbracoIcons.DataType;

        [ConfigurationField(typeof(ManualNotesConfigurationField))]
        public string Notes { get; set; }

        [ConfigurationField(typeof(ItemsConfigurationField))]
        public IEnumerable<DataListItem> Items { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            return Items;
        }

        class ManualNotesConfigurationField : NotesConfigurationField
        {
            public ManualNotesConfigurationField()
                : base(@"<p class='alert alert-warning'><strong>A note manually configuring the data list.</strong><br>
If you intend to use this Data List exclusively with a checkbox, dropdown or radiobutton list type, then you may want to consider not using a Data List editor.<br>
Data List has an overhead <i>(albeit a small overhead)</i> when processing the data source. From a performance perspective it would be better to use a specific Checkbox List, Dropdown List or Radiobutton List editor.</p>", true)
            { }
        }

        class ItemsConfigurationField : ConfigurationField
        {
            public const string Items = "Items";

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

                Key = Items;
                Name = "Options";
                Description = "Configure the option items for the dropdown list.<br><br>If you use duplicate values, then only the first option item will be used.";
                View = IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>()
                {
                    { DataTableConfigurationEditor.FieldItems, listFields },
                    { MaxItemsConfigurationField.MaxItems, 0 },
                    { DisableSortingConfigurationField.DisableSorting, Constants.Values.False },
                    { DataTableConfigurationEditor.RestrictWidth, Constants.Values.True },
                    { DataTableConfigurationEditor.UsePrevalueEditors, Constants.Values.True }
                };
            }
        }
    }
}
