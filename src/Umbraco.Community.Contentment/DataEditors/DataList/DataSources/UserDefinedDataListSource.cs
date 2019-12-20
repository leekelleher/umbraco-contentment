/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UserDefinedDataListSource : IDataListSource
    {
        public string Name => "User-defined List";

        public string Description => "Manually configure the items for the data source.";

        public string Icon => Core.Constants.Icons.DataType;

        public Dictionary<string, object> DefaultValues => default;

        [ConfigurationField(typeof(ItemsConfigurationField))]
        public IEnumerable<DataListItem> Items { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            return Items;
        }

        class ItemsConfigurationField : ConfigurationField
        {
            internal const string Items = "items";

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
                Description = "Configure the option items for the data list.<br>If you use duplicate values, then only the first option item will be used.";
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
