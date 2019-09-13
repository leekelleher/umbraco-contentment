/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public class ConfigurationEditorConfigurationEditor : ConfigurationEditor
    {
        private readonly ConfigurationEditorService _service;

        public const string Items = "items";
        public const string OrderBy = "orderBy";
        public const string OverlayView = "overlayView";

        public ConfigurationEditorConfigurationEditor()
            : base()
        {
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
                    { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.False },
                });

            Fields.Add(new EnableFilterConfigurationField());
            Fields.Add(new OverlaySizeConfigurationField());
            Fields.AddMaxItems();
            Fields.AddDisableSorting();
            Fields.AddHideLabel();
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
