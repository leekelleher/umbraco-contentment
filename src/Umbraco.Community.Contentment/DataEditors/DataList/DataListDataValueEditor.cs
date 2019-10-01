/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Composing;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Serialization;

namespace Umbraco.Community.Contentment.DataEditors
{
    public class DataListDataValueEditor : DataValueEditor
    {
        public DataListDataValueEditor(DataEditorAttribute attribute)
            : base(attribute)
        { }

        public override object Configuration
        {
            get => base.Configuration;
            set
            {
                base.Configuration = value;

                // NOTE: I'd have preferred to have done this in `DataListConfigurationEditor.ToValueEditor`, but unfortunately I couldn't alter the `View` from there.
                // Furthermore this method is triggered before `ToValueEditor`, and there's nowhere else I could manipulate the configuration values. [LK]
                if (value is Dictionary<string, object> config && config.ContainsKey(DataListConfigurationEditor.EditorConfig) == false)
                {
                    var editorConfig = new Dictionary<string, object>();

                    var serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
                    {
                        ContractResolver = new ConfigurationFieldContractResolver(),
                        Converters = new List<JsonConverter>(new[] { new FuzzyBooleanConverter() })
                    });

                    if (config.TryGetValue(DataListConfigurationEditor.DataSource, out var dataSource) &&
                        dataSource is JArray array1 &&
                        array1.Count > 0)
                    {
                        var item = array1[0];

                        var type = TypeFinder.GetTypeByName(item.Value<string>("type"));
                        if (type != null)
                        {
                            var source = item["value"].ToObject(type, serializer) as IDataListSource;
                            var items = source?.GetItems() ?? Enumerable.Empty<DataListItem>();

                            editorConfig.Add(DataListConfigurationEditor.Items, items);
                        }
                    }

                    if (config.TryGetValue(DataListConfigurationEditor.ListEditor, out var listEditor) &&
                        listEditor is JArray array2 &&
                        array2.Count > 0)
                    {
                        var item = array2[0];

                        var type = TypeFinder.GetTypeByName(item.Value<string>("type"));
                        if (type != null)
                        {
                            var val = item["value"] as JObject;
                            var obj = val.ToObject(type, serializer) as IDataListEditor;

                            View = obj.View;

                            foreach (var prop in val)
                            {
                                if (editorConfig.ContainsKey(prop.Key) == false)
                                {
                                    editorConfig.Add(prop.Key, prop.Value);
                                }
                            }

                            if (obj.DefaultConfig != null)
                            {
                                foreach (var prop in obj.DefaultConfig)
                                {
                                    if (editorConfig.ContainsKey(prop.Key) == false)
                                    {
                                        editorConfig.Add(prop.Key, prop.Value);
                                    }
                                }
                            }
                        }
                    }

                    config.Add(DataListConfigurationEditor.EditorConfig, editorConfig);
                }
            }
        }
    }
}
