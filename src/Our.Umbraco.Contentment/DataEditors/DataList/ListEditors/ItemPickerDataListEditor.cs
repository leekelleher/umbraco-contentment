/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class ItemPickerDataListEditor : IDataListEditor
    {
        public string Name => "Item Picker";

        public string Description => "Select multiple values from an Umbraco style overlay.";

        public string Icon => ItemPickerDataEditor.DataEditorIcon;

        public Dictionary<string, object> DefaultConfig => new Dictionary<string, object>
        {
            { ItemPickerConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorOverlayViewPath) },
            // TODO: [LK:2019-07-16] Consider if we should have an option for determining the default sort order (of the overlay items).
            { "overlayOrderBy", string.Empty },
        };

        public string View => IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorViewPath);

        [ConfigurationField(typeof(IconPickerConfigurationEditor.DefaultIconConfigurationField))]
        public string DefaultIcon { get; set; }

        [ConfigurationField(typeof(ItemPickerTypeConfigurationField))]
        public string ListType { get; set; }

        [ConfigurationField(typeof(MaxItemsConfigurationField))]
        public int MaxItems { get; set; }

        [ConfigurationField(typeof(AllowDuplicatesConfigurationField))]
        public bool AllowDuplicates { get; set; }

        [ConfigurationField(typeof(DisableSortingConfigurationField))]
        public bool DisableSorting { get; set; }
    }
}
