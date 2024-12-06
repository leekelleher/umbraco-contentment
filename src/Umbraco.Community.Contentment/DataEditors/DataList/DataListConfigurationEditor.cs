/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Text.Json;
using System.Text.Json.Nodes;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class DataListConfigurationEditor : ConfigurationEditor
    {
        internal const string DataSource = "dataSource";
        internal const string ListEditor = "listEditor";
        internal const string Preview = "preview";

        private readonly ConfigurationEditorUtility _utility;

        public DataListConfigurationEditor(ConfigurationEditorUtility utility)
            : base()
        {
            _utility = utility;
        }

        // TODO: [LK] Check if `ToValueEditor` is still being called/used by the backoffice.
        public override IDictionary<string, object> ToValueEditor(IDictionary<string, object> configuration)
        {
            var config = base.ToValueEditor(configuration);

            var toValueEditor = new Dictionary<string, object>();

            if (config.TryGetValueAs(DataSource, out JsonArray? array1) == true &&
                array1?.Count > 0 &&
                array1[0] is JsonObject item1 &&
                item1.GetValueAsString("key") is string key1)
            {
                var source = _utility.GetConfigurationEditor<IDataListSource>(key1);
                if (source != null)
                {
                    var sourceConfig = item1["value"]?.Deserialize<Dictionary<string, object>>();
                    if (sourceConfig is not null)
                    {
                        var items = source?.GetItems(sourceConfig) ?? Array.Empty<DataListItem>();

                        toValueEditor.Add(Constants.Conventions.ConfigurationFieldAliases.Items, items);
                    }
                }
            }

            if (config.TryGetValueAs(ListEditor, out JsonArray? array2) == true &&
                array2?.Count > 0 &&
                array2[0] is JsonObject item2 &&
                item2.GetValueAsString("key") is string key2)
            {
                var editor = _utility.GetConfigurationEditor<IDataListEditor>(key2);
                if (editor != null)
                {
                    var editorConfig = item2["value"]?.Deserialize<Dictionary<string, object>>();
                    if (editorConfig != null)
                    {
                        foreach (var prop in editorConfig)
                        {
                            if (toValueEditor.ContainsKey(prop.Key) == false)
                            {
                                toValueEditor.Add(prop.Key, prop.Value);
                            }
                        }
                    }

                    if (editor.DefaultConfig != null)
                    {
                        foreach (var prop in editor.DefaultConfig)
                        {
                            if (toValueEditor.ContainsKey(prop.Key) == false)
                            {
                                toValueEditor.Add(prop.Key, prop.Value);
                            }
                        }
                    }
                }
            }

            return toValueEditor;
        }
    }
}
