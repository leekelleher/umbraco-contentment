/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
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
using Umbraco.Core.Serialization;

namespace Umbraco.Community.Contentment.DataEditors
{
    public class DataListConfigurationEditor : ConfigurationEditor
    {
        public const string DataSource = "dataSource";
        public const string Items = "items";
        public const string ListEditor = "listEditor";

        public DataListConfigurationEditor()
            : base()
        {
            var defaultConfigEditorConfig = new Dictionary<string, object>
            {
                { MaxItemsConfigurationField.MaxItems, 1 },
                { DisableSortingConfigurationField.DisableSorting, Constants.Values.True },
                { OverlaySizeConfigurationField.OverlaySize, OverlaySizeConfigurationField.Large },
                { ConfigurationEditorConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) },
                { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.False },
            };

            var service = new ConfigurationEditorService();
            var dataSources = service.GetConfigurationEditors<IDataListSource>();
            var listEditors = service.GetConfigurationEditors<IDataListEditor>();

            Fields.Add(
                DataSource,
                "Data source",
                "Select and configure the data source.",
                IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorViewPath),
                new Dictionary<string, object>(defaultConfigEditorConfig)
                {
                    { ConfigurationEditorConfigurationEditor.Items, dataSources }
                });

            Fields.Add(
                ListEditor,
                "List editor",
                "Select and configure the type of editor for the data list.",
                IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorViewPath),
                new Dictionary<string, object>(defaultConfigEditorConfig)
                {
                    { ConfigurationEditorConfigurationEditor.Items, listEditors }
                });
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValue(DataSource, out var dataSource) && dataSource is JArray array && array.Count > 0)
            {
                var item = array[0];

                var type = TypeFinder.GetTypeByName(item.Value<string>("type"));
                if (type != null)
                {
                    var serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
                    {
                        ContractResolver = new ConfigurationFieldContractResolver(),
                        Converters = new List<JsonConverter>(new[] { new FuzzyBooleanConverter() })
                    });

                    var source = item["value"].ToObject(type, serializer) as IDataListSource;
                    var options = source?.GetItems() ?? Enumerable.Empty<DataListItem>();

                    config.Add(Items, options);
                }

                config.Remove(DataSource);
            }

            if (config.ContainsKey(ListEditor))
            {
                config.Remove(ListEditor);
            }

            return config;
        }
    }
}
