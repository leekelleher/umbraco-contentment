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
    // TODO: Consider whether this data source is necessary. [LK:2019-08-16]
    [global::Umbraco.Core.Composing.HideFromTypeFinder]
#endif
    internal class ManualDataListSource : IDataListSource
    {
        public string Name => "User-defined list";

        public string Description => "Manually configure the items for the data source.";

        public string Icon => UmbracoIcons.DataType;

        [ConfigurationField(typeof(ItemsConfigurationField))]
        public IEnumerable<DataListItem> Items { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            return Items;
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
                Description = "Configure the option items for the data list.<br><br>If you use duplicate values, then only the first option item will be used.";
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
