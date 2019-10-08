/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ItemPickerConfigurationEditor : ConfigurationEditor
    {
        internal const string DefaultIcon = "defaultIcon";
        internal const string DefaultValue = "defaultValue";
        internal const string Items = "items";
        internal const string OverlayView = "overlayView";
        internal const string OverlayOrderBy = "overlayOrderBy";

        public ItemPickerConfigurationEditor()
            : base()
        {
            var listFields = new[]
            {
                new ConfigurationField
                {
                    Key = "icon",
                    Name = "Icon",
                    View = IOHelper.ResolveUrl(IconPickerDataEditor.DataEditorViewPath),
                    Config = new Dictionary<string, object>
                    {
                        { IconPickerSizeConfigurationField.Size, IconPickerSizeConfigurationField.Small }
                    }
                },
                new ConfigurationField
                {
                    Key = "name",
                    Name = "Name",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "value",
                    Name = "Value",
                    View = "textbox"
                },
            };

            Fields.Add(
                Items,
                nameof(Items),
                "Configure the items for the item picker.",
                IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { DataTableConfigurationEditor.FieldItems, listFields },
                    { MaxItemsConfigurationField.MaxItems, 0 },
                    { DisableSortingConfigurationField.DisableSorting, Constants.Values.False },
                    { DataTableConfigurationEditor.RestrictWidth, Constants.Values.True },
                    { DataTableConfigurationEditor.UsePrevalueEditors, Constants.Values.False }
                });

            Fields.Add(new EnableFilterConfigurationField());
            Fields.Add(new ItemPickerTypeConfigurationField());
            Fields.Add(new MaxItemsConfigurationField());
            Fields.Add(new AllowDuplicatesConfigurationField());
            Fields.Add(new EnableMultipleConfigurationField());
            Fields.Add(new DisableSortingConfigurationField());
            Fields.Add(new EnableDevModeConfigurationField());
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.ContainsKey(OverlayView) == false)
            {
                config.Add(OverlayView, IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorOverlayViewPath));
            }

            return config;
        }
    }
}
