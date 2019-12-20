/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class ItemPickerDataListEditor : IDataListEditor
    {
        public string Name => "Item Picker";

        public string Description => "Select items from an Umbraco style overlay.";

        public string Icon => ItemPickerDataEditor.DataEditorIcon;

        public Dictionary<string, object> DefaultValues => default;

        public Dictionary<string, object> DefaultConfig => new Dictionary<string, object>
        {
            { ItemPickerConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorOverlayViewPath) },
            { ItemPickerConfigurationEditor.OverlayOrderBy, string.Empty },
        };

        public string View => IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorViewPath);

        [ConfigurationField(typeof(DefaultIconConfigurationField))]
        public string DefaultIcon { get; set; }

        [ConfigurationField(typeof(ItemPickerTypeConfigurationField))]
        public string ListType { get; set; }

        [ConfigurationField(typeof(MaxItemsConfigurationField))]
        public int MaxItems { get; set; }

        [ConfigurationField(typeof(AllowDuplicatesConfigurationField))]
        public bool AllowDuplicates { get; set; }

        [ConfigurationField(typeof(EnableMultipleConfigurationField))]
        public bool EnableMultiple { get; set; }

        [ConfigurationField(typeof(DisableSortingConfigurationField))]
        public bool DisableSorting { get; set; }
    }
}
