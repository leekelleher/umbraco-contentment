/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ConfigurationEditorConfigurationEditor : ConfigurationEditor
    {
        private readonly ConfigurationEditorService _service;

        internal const string Items = "items";
        internal const string OrderBy = "orderBy";
        internal const string OverlayView = "overlayView";

        public ConfigurationEditorConfigurationEditor()
            : base()
        {
            _service = new ConfigurationEditorService();

            var items = _service
                .GetConfigurationEditors<IConfigurationEditorItem>(onlyPublic: true, ignoreFields: true)
                .Select(x => new DataListItem
                {
                    Icon = x.Icon,
                    Name = x.Name,
                    Description = x.Description,
                    Value = x.Type
                });

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
            Fields.Add(new MaxItemsConfigurationField());
            Fields.Add(new DisableSortingConfigurationField());
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
