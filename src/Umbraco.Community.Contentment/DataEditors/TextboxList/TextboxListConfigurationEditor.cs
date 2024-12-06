/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class TextboxListConfigurationEditor : ConfigurationEditor
    {
        internal const string DataSource = "dataSource";

        private readonly ConfigurationEditorUtility _utility;

        public TextboxListConfigurationEditor(ConfigurationEditorUtility utility, IIOHelper ioHelper)
            : base()
        {
            _utility = utility;

//            var dataSources = new List<ConfigurationEditorModel>(_utility.GetConfigurationEditorModels<IDataListSource>());

//            Fields.Add(new ContentmentConfigurationField
//            {
//                Key = DataSource,
//                Name = "Data source",
//                Description = "Select and configure a data source.",
//                View = ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorViewPath),
//                Config = new Dictionary<string, object>
//                {
//                    { Constants.Conventions.ConfigurationFieldAliases.AddButtonLabelKey, "contentment_configureDataSource" },
//                    { MaxItemsConfigurationField.MaxItems, 1 },
//                    { DisableSortingConfigurationField.DisableSorting, Constants.Values.True },
//                    { Constants.Conventions.ConfigurationFieldAliases.OverlayView, ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) ?? string.Empty },
//                    { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
//                    { EnableFilterConfigurationField.EnableFilter, dataSources.Count > 10 ? Constants.Values.True : Constants.Values.False },
//                    { Constants.Conventions.ConfigurationFieldAliases.Items, dataSources },
//                    { "help", new {
//                        @class = "alert alert-info",
//                        title = "Do you need a custom data-source?",
//                        notes = $@"<p>If one of the data-sources above does not fit your needs, you can extend Data List with your own custom data source.</p>
//<p>To do this, read the documentation on <a href=""{Constants.Internals.RepositoryUrl}/blob/develop/docs/editors/data-list.md#extending-with-your-own-custom-data-source"" target=""_blank"" rel=""noopener""><strong>extending with your own custom data source</strong></a>.</p>" } },
//                }
//            });

//            DefaultConfiguration.Add("defaultIcon", UmbConstants.Icons.DefaultIcon);

//            Fields.Add(new ContentmentConfigurationField
//            {
//                Key = "defaultIcon",
//                Name = "Default icon",
//                Description = "Select an icon to be displayed as the default icon,<br><em>(for when no icon is available)</em>.",
//                View = ioHelper.ResolveRelativeOrVirtualUrl("~/umbraco/views/propertyeditors/listview/icon.prevalues.html"),
//            });

//            Fields.Add(new ContentmentConfigurationField
//            {
//                Key = "labelStyle",
//                Name = "Label style",
//                Description = "Select the style of the item's label.",
//                View = ioHelper.ResolveRelativeOrVirtualUrl(RadioButtonListDataListEditor.DataEditorViewPath),
//                Config = new Dictionary<string, object>
//                {
//                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
//                        {
//                            new DataListItem { Name = "Icon and Text", Value = "both", Description = "Displays both the item's icon and name." },
//                            new DataListItem { Name = "Icon only", Value = "icon", Description = "Hides the item's name and only displays the icon." },
//                            new DataListItem { Name = "Text only", Value = "text", Description = "Hides the item's icon and only displays the name." },
//                        }
//                    },
//                    { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, "both" },
//                    { ShowDescriptionsConfigurationField.ShowDescriptions, Constants.Values.True },
//                }
//            });

//            Fields.Add(new EnableDevModeConfigurationField());

//            Fields.Add(new ContentmentConfigurationField
//            {
//                Key = "preview",
//                Name = "Preview",
//                View = ioHelper.ResolveRelativeOrVirtualUrl(DataListDataEditor.DataEditorDataSourcePreviewViewPath)
//            });
        }

        public override IDictionary<string, object> ToValueEditor(IDictionary<string, object> configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValueAs(DataSource, out JArray? array) == true &&
                array?.Count > 0 &&
                array[0] is JObject item &&
                item.Value<string>("key") is string key)
            {
                var source = _utility.GetConfigurationEditor<IDataListSource>(key);
                if (source != null)
                {
                    var sourceConfig = item["value"]?.ToObject<Dictionary<string, object>>();
                    if (sourceConfig is not null)
                    {
                        var items = source?.GetItems(sourceConfig) ?? Array.Empty<DataListItem>();

                        config.Add(Constants.Conventions.ConfigurationFieldAliases.Items, items);
                    }

                    _ = config.Remove(DataSource);
                }
            }

            return config;
        }
    }
}
