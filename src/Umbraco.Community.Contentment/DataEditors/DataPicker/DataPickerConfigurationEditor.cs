/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class DataPickerConfigurationEditor : ConfigurationEditor
    {
        internal const string DataSource = "dataSource";
        internal const string DisplayMode = "displayMode";

        private readonly IIOHelper _ioHelper;
        private readonly ConfigurationEditorUtility _utility;

        public DataPickerConfigurationEditor(
            IIOHelper ioHelper,
            ConfigurationEditorUtility utility)
            : base()
        {
            _ioHelper = ioHelper;
            _utility = utility;

            var configEditorViewPath = ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorViewPath);
            var defaultConfigEditorConfig = new Dictionary<string, object>
            {
                { MaxItemsConfigurationField.MaxItems, 1 },
                { DisableSortingConfigurationField.DisableSorting, Constants.Values.True },
                { Constants.Conventions.ConfigurationFieldAliases.OverlayView, ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) ?? string.Empty },
                { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
            };

            var dataSources = new List<ConfigurationEditorModel>(utility.GetConfigurationEditorModels<IDataPickerSource>());
            var displayModes = new List<ConfigurationEditorModel>(utility.GetConfigurationEditorModels<IDataPickerDisplayMode>());

            // NOTE: Sets the default display mode to be the Cards.
            var defaultDisplayMode = displayModes.FirstOrDefault(x => x.Key.InvariantEquals(typeof(CardsDataPickerDisplayMode).GetFullNameWithAssembly()));
            if (defaultDisplayMode != null)
            {
                DefaultConfiguration.Add(DisplayMode, new[] { new { key = defaultDisplayMode.Key, value = defaultDisplayMode.DefaultValues } });
            }

            var defaultOverlaySize = "medium";

            DefaultConfiguration.Add("pageSize", 12);
            DefaultConfiguration.Add("overlaySize", defaultOverlaySize);
            DefaultConfiguration.Add(MaxItemsConfigurationField.MaxItems, 0);

            Fields.Add(new ConfigurationField
            {
                Key = DataSource,
                Name = "Data source",
                Description = "Select and configure a data source.",
                View = configEditorViewPath,
                Config = new Dictionary<string, object>(defaultConfigEditorConfig)
                {
                    { Constants.Conventions.ConfigurationFieldAliases.AddButtonLabelKey, "contentment_configureDataSource" },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, dataSources },
                    { EnableFilterConfigurationField.EnableFilter, dataSources.Count > 10 ? Constants.Values.True : Constants.Values.False },
                    { "help", new {
                        @class = "alert alert-info",
                        title = "Do you need a custom data-source?",
                        notes = $@"<p>If one of the data-sources above does not fit your needs, you can extend Data Picker with your own custom data source.</p>
<p>To do this, read the documentation on <a href=""{Constants.Internals.RepositoryUrl}/blob/develop/docs/editors/data-picker.md#extending-with-your-own-custom-data-source"" target=""_blank"" rel=""noopener""><strong>extending with your own custom data source</strong></a>.</p>" } },
                }
            });

            Fields.Add(new ConfigurationField
            {
                Key = DisplayMode,
                Name = "Display mode",
                Description = "Select display mode for the picker editor.",
                View = configEditorViewPath,
                Config = new Dictionary<string, object>(defaultConfigEditorConfig)
                {
                    { Constants.Conventions.ConfigurationFieldAliases.AddButtonLabelKey, "contentment_configureDisplayMode" },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, displayModes },
                }
            });

            Fields.Add(new ConfigurationField
            {
                Key = "pageSize",
                Name = "Page size",
                Description = "How many items to display per page? The default value is 12.",
                View = ioHelper.ResolveRelativeOrVirtualUrl(NumberInputDataEditor.DataEditorViewPath)
            });

            Fields.Add(new ConfigurationField
            {
                Key = "overlaySize",
                Name = "Editor overlay size",
                Description = $"Select the size of the overlay panel. The default is '{defaultOverlaySize}'.",
                View = ioHelper.ResolveRelativeOrVirtualUrl(ButtonsDataListEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "Small", Value = "small" },
                            new DataListItem { Name = "Medium", Value = "medium" },
                            new DataListItem { Name = "Large", Value = "large" }
                        }
                    },
                    { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, defaultOverlaySize },
                    { "labelStyle", "text" },
                    { "size", "m" },
                }
            });

            Fields.Add(new ConfigurationField
            {
                Key = "hideSearch",
                Name = "Hide search box?",
                Description = "Hide the search box in the overlay panel.",
                View = "boolean",
            });

            Fields.Add(new MaxItemsConfigurationField(ioHelper));
            Fields.Add(new EnableDevModeConfigurationField());
        }

        public override IDictionary<string, object> ToValueEditor(IDictionary<string, object> configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValueAs(DisplayMode, out JArray? array1) == true &&
                array1?.Count > 0 &&
                array1[0] is JObject item1 &&
                item1.Value<string>("key") is string key1)
            {
                var displayMode = _utility.GetConfigurationEditor<IDataPickerDisplayMode>(key1);
                if (displayMode != null)
                {
                    // NOTE: Removing the raw configuration as the display mode may have the same key.
                    _ = config.Remove(DisplayMode);

                    var editorConfig = item1["value"]?.ToObject<Dictionary<string, object>>();
                    if (editorConfig != null)
                    {
                        foreach (var prop in editorConfig)
                        {
                            if (config.ContainsKey(prop.Key) == false)
                            {
                                config.Add(prop.Key, prop.Value);
                            }
                        }
                    }

                    if (displayMode.DefaultConfig != null)
                    {
                        foreach (var prop in displayMode.DefaultConfig)
                        {
                            if (config.ContainsKey(prop.Key) == false)
                            {
                                config.Add(prop.Key, prop.Value);
                            }
                        }
                    }
                }
            }

            if (config.ContainsKey(Constants.Conventions.ConfigurationFieldAliases.OverlayView) == false)
            {
                config.Add(Constants.Conventions.ConfigurationFieldAliases.OverlayView, _ioHelper.ResolveRelativeOrVirtualUrl(DataPickerDataEditor.DataEditorOverlayViewPath) ?? string.Empty);
            }

            return config;
        }
    }
}
