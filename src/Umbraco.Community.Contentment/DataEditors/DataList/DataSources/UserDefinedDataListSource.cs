/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Core.Composing.HideFromTypeFinder]
    internal sealed class UserDefinedDataListSource : IDataListSource
    {
        private readonly ConfigurationField[] _listFields;

        public UserDefinedDataListSource()
        {
            _listFields = new[]
            {
                new ConfigurationField
                {
                    Key = "icon",
                    Name = "Icon",
                    View = IOHelper.ResolveUrl("~/umbraco/views/propertyeditors/listview/icon.prevalues.html")
                },
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
        }

        public string Name => "User-defined List";

        public string Description => "Manually configure the items for the data source.";

        public string Icon => Core.Constants.Icons.DataType;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "items",
                Name = "Options",
                Description = "Configure the option items for the data list.<br>If you use duplicate values, then only the first option item will be used.",
                View = IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>()
                {
                    { DataTableConfigurationEditor.FieldItems, _listFields },
                    { MaxItemsConfigurationField.MaxItems, 0 },
                    { DisableSortingConfigurationField.DisableSorting, Constants.Values.False },
                    { DataTableConfigurationEditor.RestrictWidth, Constants.Values.True },
                    { DataTableConfigurationEditor.UsePrevalueEditors, Constants.Values.True }
                },
            }
        };

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Large;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return config.TryGetValueAs("items", out JArray array)
                ? array.ToObject<IEnumerable<DataListItem>>()
                : Enumerable.Empty<DataListItem>();
        }
    }
}
