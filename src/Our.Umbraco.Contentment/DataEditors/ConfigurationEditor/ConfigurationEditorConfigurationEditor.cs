/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using UmbIcons = Umbraco.Core.Constants.Icons;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class ConfigurationEditorConfigurationEditor : ConfigurationEditor
    {
        private readonly ConfigurationEditorService _service;

        public const string EnableDevMode = "enableDevMode";
        public const string EnableFilter = "enableFilter";
        public const string Items = "items";
        public const string OrderBy = "orderBy";
        public const string OverlaySize = "overlaySize";
        public const string OverlayView = "overlayView";

        public ConfigurationEditorConfigurationEditor()
            : base()
        {
            // TODO: [LK:2019-07-19] Consider using DI for ConfigurationEditorService.
            _service = new ConfigurationEditorService();

            var configEditors = _service.GetConfigurationEditors<IConfigurationEditorItem>(onlyPublic: true, ignoreFields: true);
            var items = new List<DataListItem>();

            foreach (var configEditor in configEditors)
            {
                items.Add(new DataListItem
                {
                    Icon = configEditor.Icon,
                    Name = configEditor.Name,
                    Description = configEditor.Description,
                    Value = configEditor.Type
                });
            }

            Fields.Add(
                Items,
                nameof(Items),
                "Select the configuration editors to use.",
                IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorViewPath),
                new Dictionary<string, object>
                {
                    { AllowDuplicatesConfigurationField.AllowDuplicates, Constants.Values.False },
                    { ItemPickerConfigurationEditor.Items, items },
                    { ItemPickerTypeConfigurationField.ListType, ItemPickerTypeConfigurationField.List },
                    { ItemPickerConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorOverlayViewPath) },
                    { ItemPickerConfigurationEditor.EnableDevMode, Constants.Values.False },
                });

            Fields.Add(
                EnableFilter,
                "Enable filter?",
                "Select to enable the search filter in the overlay selection panel.",
                "boolean");

            Fields.Add(new OverlaySizeConfigurationField());
            Fields.AddMaxItems();
            Fields.AddDisableSorting();
            Fields.AddHideLabel();
        }

        internal class OverlaySizeConfigurationField : ConfigurationField
        {
            public const string Small = "small";
            public const string Large = "large";

            public OverlaySizeConfigurationField()
                : base()
            {
                var items = new[]
                {
                    new { name = nameof(Small), value = Small },
                    new { name = nameof(Large), value = Large }
                };

                Key = OverlaySize;
                Name = "Overlay size";
                Description = "Select the size of the overlay editing panel. By default this is set to 'large'. However if the configuration editor fields require a smaller panel, select 'small'.";
                View = IOHelper.ResolveUrl(RadioButtonListDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    { OrientationConfigurationField.Orientation, OrientationConfigurationField.Vertical },
                    { RadioButtonListConfigurationEditor.Items, items },
                    { RadioButtonListConfigurationEditor.DefaultValue, Large }
                };
            }
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValue(Items, out var items) && items is JArray array && array.Count > 0)
            {
                var types = new List<Type>();

                foreach (var item in array)
                {
                    var type = TypeFinder.GetTypeByName(item.Value<string>());
                    if (type != null)
                    {
                        types.Add(type);
                    }
                }

                config[Items] = _service.GetConfigurationEditors<IConfigurationEditorItem>(types);
                config[OrderBy] = string.Empty; // Set to empty, so to preserve the selected order.
            }

            if (config.ContainsKey(OverlayView) == false)
            {
                config.Add(OverlayView, IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath));
            }

            return config;
        }
    }
}
