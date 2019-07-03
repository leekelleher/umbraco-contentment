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

namespace Our.Umbraco.Contentment.DataEditors
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
                { ConfigurationEditorConfigurationEditor.OverlaySize, ConfigurationEditorConfigurationEditor.OverlaySizeConfigurationField.Large },
                { ConfigurationEditorConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) },
                { ConfigurationEditorConfigurationEditor.EnableDevMode, Constants.Values.True },
            };

            var dataSources = GetDataSources();
            var listEditors = GetListEditors();

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

            Fields.AddHideLabel();
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValue(DataSource, out var dataSource) && dataSource is JArray array && array.Count > 0)
            {
                var item = array[0];

                // NOTE: Using `TypeFinder` here, as `TypeLoader` doesn't expose the `GetTypeByName` method. [LK:2019-06-06]
                var type = TypeFinder.GetTypeByName(item.Value<string>("type"));
                if (type != null)
                {
                    var serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
                    {
                        // TODO: [LK:2019-06-06] How to do DI when deserializing? (I want to inject IEntityService into UmbracoEntityDataListSource)
                        // https://www.newtonsoft.com/json/help/html/DeserializeWithDependencyInjection.htm
                        ContractResolver = new ConfigurationFieldContractResolver(),
                        Converters = new List<JsonConverter>(new[] { new FuzzyBooleanConverter() })
                    });

                    // TODO: [LK:2019-07-01] Give this a try...
                    // https://our.umbraco.com/forum/extending-umbraco-and-using-the-api/97948-how-to-deserialize-json-with-lightinject-dependency-injection#comment-309016
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

        private IEnumerable<ConfigurationEditorModel> GetDataSources()
        {
            return ConfigurationEditorConfigurationEditor.GetConfigurationEditors<IDataListSource>();
        }

        private IEnumerable<ConfigurationEditorModel> GetListEditors()
        {
            return ConfigurationEditorConfigurationEditor.GetConfigurationEditors<IDataListEditor>();
        }
    }
}
