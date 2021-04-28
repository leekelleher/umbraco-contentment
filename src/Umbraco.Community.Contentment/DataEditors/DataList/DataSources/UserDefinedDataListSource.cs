/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Core.Composing.HideFromTypeFinder]
    public sealed class UserDefinedDataListSource : IDataListSource
    {
        public string Name => "User-defined List";

        public string Description => "Manually configure the items for the data source.";

        public string Icon => Core.Constants.Icons.DataType;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "items",
                Name = "Options",
                Description = "Configure the option items for the data list.<br><br>Please try to avoid using duplicate values, as this may cause adverse issues with list editors.",
                View = DataListDataEditor.DataEditorListEditorViewPath,
                Config = new Dictionary<string, object>()
                {
                    { "confirmRemoval", Constants.Values.True },
                    { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
                    { MaxItemsConfigurationField.MaxItems, 0 },
                },
            }
        };

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Large;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return config.TryGetValueAs("items", out JArray array) == true
                ? array.ToObject<IEnumerable<DataListItem>>().DistinctBy(x => x.Value)
                : Enumerable.Empty<DataListItem>();
        }
    }
}
