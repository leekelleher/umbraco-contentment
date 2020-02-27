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
    public sealed class DataListDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "DataList";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Data List";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "_empty.html";
        internal const string DataEditorIcon = "icon-bulleted-list";

        private readonly ConfigurationEditorUtility _utility;

        public DataListDataEditor(ConfigurationEditorUtility utility) => _utility = utility;

        public string Alias => DataEditorAlias;

        public EditorType Type => EditorType.PropertyValue;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => Core.Constants.PropertyEditors.Groups.Lists;

        public bool IsDeprecated => false;

        public IDictionary<string, object> DefaultConfiguration => new Dictionary<string, object>();

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor() => new DataListConfigurationEditor(_utility);

        public IDataValueEditor GetValueEditor()
        {
            return new DataValueEditor
            {
                ValueType = ValueTypes.Json,
                View = DataEditorViewPath,
            };
        }

        public IDataValueEditor GetValueEditor(object configuration)
        {
            var view = default(string);

            if (configuration is Dictionary<string, object> config)
            {
                config[DataListConfigurationEditor.EditorConfig] = GetEditorConfiguration(config);

                if (config.TryGetValue(DataListConfigurationEditor.EditorView, out var tmp) && tmp is string view1)
                {
                    view = view1;
                }
            }

            return new DataValueEditor
            {
                Configuration = configuration,
                ValueType = ValueTypes.Json,
                View = view ?? DataEditorViewPath,
            };
        }

        private Dictionary<string, object> GetEditorConfiguration(Dictionary<string, object> configuration)
        {
            var editorConfig = new Dictionary<string, object>();

            if (configuration.TryGetValue(DataListConfigurationEditor.DataSource, out var dataSource) &&
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

            if (configuration.TryGetValue(DataListConfigurationEditor.ListEditor, out var listEditor) &&
                listEditor is JArray array2 &&
                array2.Count > 0)
            {
                var item = array2[0];

                var editor = _utility.GetConfigurationEditor<IDataListEditor>(item.Value<string>("type"));
                if (editor != null)
                {
                    var val = item["value"] as JObject;

                    configuration[DataListConfigurationEditor.EditorView] = editor.View;

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

            return editorConfig;
        }
    }
}
