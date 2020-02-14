/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class DataListDataValueEditor : DataValueEditor
    {
        private readonly ConfigurationEditorUtility _utility;

        public DataListDataValueEditor(DataEditorAttribute attribute, ConfigurationEditorUtility utility)
            : base(attribute)
        {
            _utility = utility;
        }

        public override object Configuration
        {
            get => base.Configuration;
            set
            {
                base.Configuration = value;

                // NOTE: I'd have preferred to do this in `DataListConfigurationEditor.ToValueEditor`, but I couldn't alter the `View` from there.
                // ...and this method is triggered before `ToValueEditor`, and there's nowhere else I can manipulate the configuration values.
                // Maybe we need a `ConfigureViewEditor(config)` method? In Umbraco v7, we had a `ConfigureForDisplay` method. [LK]
                if (value is Dictionary<string, object> config)
                {
                    if (config.ContainsKey(DataListConfigurationEditor.EditorConfig) == false)
                    {
                        var editorConfig = new Dictionary<string, object>();

                        if (config.TryGetValue(DataListConfigurationEditor.DataSource, out var dataSource) &&
                            dataSource is JArray array1 &&
                            array1.Count > 0)
                        {
                            var item = array1[0];
                            var source = _utility.GetConfigurationEditor<IDataListSource>(item.Value<string>("type"));
                            if (source != null)
                            {
                                var sourceConfig = item["value"].ToObject<Dictionary<string, object>>();
                                var items = source?.GetItems(sourceConfig) ?? Enumerable.Empty<DataListItem>();

                                editorConfig.Add(DataListConfigurationEditor.Items, items);
                            }
                        }

                        if (config.TryGetValue(DataListConfigurationEditor.ListEditor, out var listEditor) &&
                            listEditor is JArray array2 &&
                            array2.Count > 0)
                        {
                            var item = array2[0];

                            var editor = _utility.GetConfigurationEditor<IDataListEditor>(item.Value<string>("type"));
                            if (editor != null)
                            {
                                var val = item["value"] as JObject;

                                if (config.ContainsKey(DataListConfigurationEditor.EditorView) == false)
                                {
                                    config.Add(DataListConfigurationEditor.EditorView, editor.View);
                                }

                                foreach (var prop in val)
                                {
                                    if (editorConfig.ContainsKey(prop.Key) == false)
                                    {
                                        editorConfig.Add(prop.Key, prop.Value);
                                    }
                                }

                                if (editor.DefaultConfig != null)
                                {
                                    foreach (var prop in editor.DefaultConfig)
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

                    if (config.TryGetValue(DataListConfigurationEditor.EditorView, out var tmp) && tmp is string view)
                    {
                        View = view;
                    }
                }
            }
        }
    }
}
