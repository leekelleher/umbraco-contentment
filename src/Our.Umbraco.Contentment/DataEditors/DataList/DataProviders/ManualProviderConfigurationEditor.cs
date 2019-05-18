/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class ManualProviderConfigurationEditor : IDataProvider
    {
        public string Name => "Manual";

        public string Description => "Manually configure the items for the data source.";

        public string Icon => "icon-autofill";

        [ConfigurationField(typeof(ItemsConfigurationField))]
        public IEnumerable<LabelValueModel> Items { get; set; }

        public Dictionary<string, string> GetItems()
        {
            // TODO: Review this, make it bulletproof. Need to avoid the "An item with the same key has already been added" error.

            return Items?
                .DistinctBy(x => x.value)
                .ToDictionary(x => x.value, x => x.label);
        }

        class ItemsConfigurationField : ConfigurationField
        {
            public ItemsConfigurationField()
            {
                var listFields = new[]
                {
                    new ConfigurationField
                    {
                        Key = "label",
                        Name = "Label",
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
                    { "fields", listFields },
                    { Constants.Conventions.ConfigurationEditors.MaxItems, 0 },
                    { Constants.Conventions.ConfigurationEditors.DisableSorting, Constants.Values.False },
                    { "restrictWidth", Constants.Values.True },
                    { "usePrevalueEditors", Constants.Values.True }
                };
            }
        }

        // TODO: Review if we need to separate out this object-model?
        public class LabelValueModel
        {
            public string label { get; set; }
            public string value { get; set; }
        }
    }
}
