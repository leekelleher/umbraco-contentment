/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class DataListConfigurationEditor : ConfigurationEditor
    {
        internal const string DataSource = "dataSource";
        internal const string Items = "items";
        internal const string ListEditor = "listEditor";
        internal const string EditorConfig = "editorConfig";
        internal const string EditorView = "editorView";

        private readonly ConfigurationEditorUtility _utility;
        private readonly IIOHelper _ioHelper;

        public DataListConfigurationEditor(ConfigurationEditorUtility utility, IIOHelper ioHelper, IShortStringHelper shortStringHelper)
            : base()
        {
            _utility = utility;
            _ioHelper = ioHelper;
            var configEditorViewPath = _ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorViewPath);
            var defaultConfigEditorConfig = new Dictionary<string, object>
            {
                { MaxItemsConfigurationField.MaxItems, 1 },
                { DisableSortingConfigurationField.DisableSorting, Constants.Values.True },
                { Constants.Conventions.ConfigurationFieldAliases.OverlayView, _ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) },
                { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
            };

            var dataSources = utility.GetConfigurationEditorModels<IDataListSource>(shortStringHelper).ToList();
            var listEditors = utility.GetConfigurationEditorModels<IDataListEditor>(shortStringHelper).ToList();

            Fields.Add(new ConfigurationField
            {
                Key = DataSource,
                Name = "Data source",
                Description = "Select and configure a data source.",
                View = configEditorViewPath,
                Config = new Dictionary<string, object>(defaultConfigEditorConfig)
                {
                    { Constants.Conventions.ConfigurationFieldAliases.AddButtonLabelKey, "contentment_configureDataSource" },
                    { EnableFilterConfigurationField.EnableFilter, dataSources.Count > 10 ? Constants.Values.True : Constants.Values.False },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, dataSources },
                }
            });

            Fields.Add(new ConfigurationField
            {
                Key = ListEditor,
                Name = "List editor",
                Description = "Select and configure a list editor.",
                View = configEditorViewPath,
                Config = new Dictionary<string, object>(defaultConfigEditorConfig)
                {
                    { Constants.Conventions.ConfigurationFieldAliases.AddButtonLabelKey, "contentment_configureListEditor" },
                    { EnableFilterConfigurationField.EnableFilter, dataSources.Count > 10 ? Constants.Values.True : Constants.Values.False },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, listEditors },
                }
            });

            Fields.Add(new ConfigurationField
            {
                Key = "preview",
                Name = "Preview",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(DataListDataEditor.DataEditorPreviewViewPath)
            });
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            var toValueEditor = new Dictionary<string, object>();

            if (config.TryGetValueAs(DataSource, out JArray array1) == true && array1.Count > 0 && array1[0] is JObject item1)
            {
                // NOTE: Patches a breaking-change. I'd renamed `type` to become `key`. [LK:2020-04-03]
                if (item1.ContainsKey("key") == false && item1.ContainsKey("type") == true)
                {
                    item1.Add("key", item1["type"]);
                    item1.Remove("type");
                }

                var source = _utility.GetConfigurationEditor<IDataListSource>(item1.Value<string>("key"));
                if (source != null)
                {
                    var sourceConfig = item1["value"].ToObject<Dictionary<string, object>>();
                    var items = source?.GetItems(sourceConfig) ?? Enumerable.Empty<DataListItem>();

                    toValueEditor.Add(Items, items);
                }
            }

            if (config.TryGetValueAs(ListEditor, out JArray array2) == true && array2.Count > 0 && array2[0] is JObject item2)
            {
                // NOTE: Patches a breaking-change. I'd renamed `type` to become `key`. [LK:2020-04-03]
                if (item2.ContainsKey("key") == false && item2.ContainsKey("type") == true)
                {
                    item2.Add("key", item2["type"]);
                    item2.Remove("type");
                }

                var editor = _utility.GetConfigurationEditor<IDataListEditor>(item2.Value<string>("key"));
                if (editor != null)
                {
                    var editorConfig = item2["value"].ToObject<Dictionary<string, object>>();

                    foreach (var prop in editorConfig)
                    {
                        if (toValueEditor.ContainsKey(prop.Key) == false)
                        {
                            toValueEditor.Add(prop.Key, prop.Value);
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
