/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
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

                // NOTE: I'd have preferred to do this in `DataListConfigurationEditor.ToValueEditor`, but I couldn't alter the `View` from there.
                // ...and this method is triggered before `ToValueEditor`, and there's nowhere else I can manipulate the configuration values. [LK]
                if (value is Dictionary<string, object> config &&
                    config.TryGetValue(DataListConfigurationEditor.ListEditor, out var listEditor) &&
                    listEditor is JArray array &&
                    array.Count > 0)
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

                        var val = item["value"] as JObject;
                        var obj = val.ToObject(type, serializer) as IDataListEditor;

                        View = obj.View;

                        foreach (var prop in val)
                        {
                            if (config.ContainsKey(prop.Key) == false)
                            {
                                config.Add(prop.Key, prop.Value);
                            }
                        }

                        if (obj.DefaultConfig != null)
                        {
                            foreach (var prop in obj.DefaultConfig)
                            {
                                if (config.ContainsKey(prop.Key) == false)
                                {
                                    config.Add(prop.Key, prop.Value);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
