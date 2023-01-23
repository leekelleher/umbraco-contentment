/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
#if NET472
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Strings;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class DataPickerConfigurationEditor : ConfigurationEditor
    {
        internal const string DataSource = "dataSource";
        internal const string DisplayMode = "displayMode";

        public DataPickerConfigurationEditor(
            IIOHelper ioHelper,
            IShortStringHelper shortStringHelper,
            ConfigurationEditorUtility utility)
            : base()
        {
            var displayModes = utility
                .GetConfigurationEditorModels<IContentBlocksDisplayMode>(shortStringHelper)
                .Select(x => new DataListItem
                {
                    Name = x.Name,
                    Value = x.Name.ToSafeAlias(shortStringHelper, true),
                    Icon = x.Icon,
                    Description = x.Description,
                });

            var dataSources = new List<ConfigurationEditorModel>(utility.GetConfigurationEditorModels<IDataPickerSource>(shortStringHelper));

            var defaultOverlaySize = "medium";

            DefaultConfiguration.Add(DisplayMode, "cards");
            DefaultConfiguration.Add("maxItems", 0);
            DefaultConfiguration.Add("overlaySize", defaultOverlaySize);
            DefaultConfiguration.Add("pageSize", 12);

            Fields.Add(new ConfigurationField
            {
                Key = DisplayMode,
                Name = "Display mode",
                Description = "Select display mode for the picker editor.",
                View = ioHelper.ResolveRelativeOrVirtualUrl("~/App_Plugins/Contentment/editors/item-picker.html"),
                Config = new Dictionary<string, object>
                {
                    { "defaultValue", "cards" },
                    { "items", displayModes },
                    { "maxItems", 1 },
                    { "disableSorting", "1" },
                    { "listType", "list" },
                    { "overlayView", ioHelper.ResolveRelativeOrVirtualUrl("~/App_Plugins/Contentment/editors/item-picker.overlay.html") ?? string.Empty },
                }
            });

            Fields.Add(new ConfigurationField
            {
                Key = DataSource,
                Name = "Data source",
                Description = "Select and configure a data source.",
                View = ioHelper.ResolveRelativeOrVirtualUrl("~/App_Plugins/Contentment/editors/configuration-editor.html"),
                Config = new Dictionary<string, object>
                {
                    { "addButtonLabelKey", "contentment_configureDataSource" },
                    { "disableSorting", "1" },
                    { "enableDevMode", "1" },
                    { "enableFilter", (dataSources.Count > 10) ? "1" : "0" },
                    { "items", dataSources },
                    { "maxItems", 1 },
                    { "overlayView", ioHelper.ResolveRelativeOrVirtualUrl("~/App_Plugins/Contentment/editors/configuration-editor.overlay.html") ?? string.Empty },
                }
            });

            Fields.Add(new ConfigurationField
            {
                Key = "maxItems",
                Name = "Maximum items",
                Description = "Enter the number for the maximum items allowed.<br>Use '0' for an unlimited amount.",
                View = ioHelper.ResolveRelativeOrVirtualUrl("~/App_Plugins/Contentment/editors/number-input.html")
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
                Key = "pageSize",
                Name = "Page size",
                Description = "How many items to display per page? The default value is 12.",
                View = ioHelper.ResolveRelativeOrVirtualUrl("~/App_Plugins/Contentment/editors/number-input.html")
            });

            Fields.Add(new ConfigurationField
            {
                Key = "enableDevMode",
                Name = "Developer mode?",
                Description = "Enable a property action to edit the raw data for the editor value.",
                View = "boolean",
            });
        }
    }
}
