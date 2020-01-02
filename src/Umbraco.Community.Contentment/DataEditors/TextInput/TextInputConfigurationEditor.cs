/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class TextInputConfigurationEditor : ConfigurationEditor
    {
        public const string Items = "items";

        public TextInputConfigurationEditor(ConfigurationEditorUtility utility)
            : base()
        {
            var dataSources = utility.GetConfigurationEditors<IDataListSource>();

            Fields.Add(new PlaceholderTextConfigurationField());
            Fields.Add(new AutocompleteConfigurationField());
            Fields.Add(new MaxCharsConfigurationField());
            Fields.Add(
                Items,
                "Data list",
                "<em>(optional)</em> Select and configure the data source to provide a HTML5 &lt;datalist&gt; for this text input.",
                IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { OverlaySizeConfigurationField.OverlaySize, OverlaySizeConfigurationField.Small },
                    { ConfigurationEditorConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) },
                    { ConfigurationEditorConfigurationEditor.Items, dataSources },
                    { MaxItemsConfigurationField.MaxItems, 1 },
                    { DisableSortingConfigurationField.DisableSorting, Constants.Values.True },
                    { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
                });
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValue(Items, out var items) && items is JArray array && array.Count > 0)
            {
                var item = array[0];

                var type = TypeFinder.GetTypeByName(item.Value<string>("type"));
                if (type != null)
                {
                    var serializer = JsonSerializer.CreateDefault(new Serialization.ConfigurationFieldJsonSerializerSettings());

                    var source = item["value"].ToObject(type, serializer) as IDataListSource;
                    var options = source?.GetItems() ?? Enumerable.Empty<DataListItem>();

                    config[Items] = options;
                }
            }

            return config;
        }
    }
}
