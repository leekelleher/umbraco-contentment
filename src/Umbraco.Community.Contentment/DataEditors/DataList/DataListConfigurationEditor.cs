/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class DataListConfigurationEditor : ConfigurationEditor
    {
        internal const string DataSource = "dataSource";
        internal const string ListEditor = "listEditor";
        internal const string Preview = "preview";

        private readonly ConfigurationEditorUtility _utility;

        public DataListConfigurationEditor(
            IIOHelper ioHelper,
            ILocalizedTextService localizedTextService,
            ConfigurationEditorUtility utility)
            : base()
        {
            _utility = utility;

            var configEditorViewPath = ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorViewPath);
            var defaultConfigEditorConfig = new Dictionary<string, object>
            {
                { MaxItemsConfigurationField.MaxItems, 1 },
                { DisableSortingConfigurationField.DisableSorting, Constants.Values.True },
                { Constants.Conventions.ConfigurationFieldAliases.OverlayView, ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) ?? string.Empty },
                { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
            };

            var dataSources = new List<ConfigurationEditorModel>(utility.GetConfigurationEditorModels<IDataListSource>());
            var listEditors = new List<ConfigurationEditorModel>(utility.GetConfigurationEditorModels<IDataListEditor>());

            Fields.Add(new ConfigurationField
            {
                Key = DataSource,
                Name = localizedTextService.LocalizeContentment("labelDataSource", "Data source"),
                Description = localizedTextService.LocalizeContentment("configureDataSource", "Select and configure a data source."),
                View = configEditorViewPath,
                Config = new Dictionary<string, object>(defaultConfigEditorConfig)
                {
                    { Constants.Conventions.ConfigurationFieldAliases.AddButtonLabelKey, "contentment_configureDataSource" },
                    { EnableFilterConfigurationField.EnableFilter, dataSources.Count > 10 ? Constants.Values.True : Constants.Values.False },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, dataSources },
                    { "help", new {
                        @class = "alert alert-info",
                        title = "Do you need a custom data-source?",
                        notes = $@"<p>If one of the data-sources above does not fit your needs, you can extend Data List with your own custom data source.</p>
<p>To do this, read the documentation on <a href=""{Constants.Internals.RepositoryUrl}/blob/develop/docs/editors/data-list.md#extending-with-your-own-custom-data-source"" target=""_blank"" rel=""noopener""><strong>extending with your own custom data source</strong></a>.</p>" } },
                }
            });

            Fields.Add(new ConfigurationField
            {
                Key = ListEditor,
                Name = localizedTextService.LocalizeContentment("labelListEditor", "List editor"),
                Description = localizedTextService.LocalizeContentment("configureListEditor", "Select and configure a list editor."),
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
                Key = Preview,
                Name = nameof(Preview),
                View = ioHelper.ResolveRelativeOrVirtualUrl(DataListDataEditor.DataEditorPreviewViewPath)
            });
        }

        public override IDictionary<string, object> ToValueEditor(IDictionary<string, object> configuration)
        {
            var config = base.ToValueEditor(configuration);

            var toValueEditor = new Dictionary<string, object>();

            if (config.TryGetValueAs(DataSource, out JArray? array1) == true &&
                array1?.Count > 0 &&
                array1[0] is JObject item1 &&
                item1.Value<string>("key") is string key1)
            {
                var source = _utility.GetConfigurationEditor<IDataListSource>(key1);
                if (source != null)
                {
                    var sourceConfig = item1["value"]?.ToObject<Dictionary<string, object>>();
                    if (sourceConfig is not null)
                    {
                        var items = source?.GetItems(sourceConfig) ?? Array.Empty<DataListItem>();

                        toValueEditor.Add(Constants.Conventions.ConfigurationFieldAliases.Items, items);
                    }
                }
            }

            if (config.TryGetValueAs(ListEditor, out JArray? array2) == true &&
                array2?.Count > 0 &&
                array2[0] is JObject item2 &&
                item2.Value<string>("key") is string key2)
            {
                var editor = _utility.GetConfigurationEditor<IDataListEditor>(key2);
                if (editor != null)
                {
                    var editorConfig = item2["value"]?.ToObject<Dictionary<string, object>>();
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
