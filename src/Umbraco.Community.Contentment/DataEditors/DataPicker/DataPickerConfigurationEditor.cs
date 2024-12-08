/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Text.Json;
using System.Text.Json.Nodes;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
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

            //var configEditorViewPath = ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorViewPath);
            var defaultConfigEditorConfig = new Dictionary<string, object>
            {
                { MaxItemsConfigurationField.MaxItems, 1 },
                { DisableSortingConfigurationField.DisableSorting, Constants.Values.True },
                { Constants.Conventions.ConfigurationFieldAliases.OverlayView, ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) ?? string.Empty },
                { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
            };

            //var dataSources = new List<ConfigurationEditorModel>(utility.GetConfigurationEditorModels<IDataPickerSource>());
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
        }

        public override IDictionary<string, object> ToValueEditor(IDictionary<string, object> configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValueAs(DisplayMode, out JsonArray? array1) == true &&
                array1?.Count > 0 &&
                array1[0] is JsonObject item1 &&
                item1.GetValueAsString("key") is string key1)
            {
                var displayMode = _utility.GetConfigurationEditor<IDataPickerDisplayMode>(key1);
                if (displayMode != null)
                {
                    // NOTE: Removing the raw configuration as the display mode may have the same key.
                    _ = config.Remove(DisplayMode);

                    var editorConfig = item1["value"]?.Deserialize<Dictionary<string, object>>();
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

            //if (config.ContainsKey(Constants.Conventions.ConfigurationFieldAliases.OverlayView) == false)
            //{
            //    config.Add(Constants.Conventions.ConfigurationFieldAliases.OverlayView, _ioHelper.ResolveRelativeOrVirtualUrl(DataPickerDataEditor.DataEditorOverlayViewPath) ?? string.Empty);
            //}

            return config;
        }
    }
}
