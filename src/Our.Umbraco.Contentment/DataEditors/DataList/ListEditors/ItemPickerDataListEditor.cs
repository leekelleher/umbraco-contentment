/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class ItemPickerDataListEditor : IDataListEditor
    {
        public string Name => "Item Picker";

        public string Description => "Select multiple values from an Umbraco style overlay.";

        public string Icon => ItemPickerDataEditor.DataEditorIcon;

        public string View => IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorViewPath);

        [ConfigurationField(typeof(IconPickerConfigurationEditor.DefaultIconConfigurationField))]
        public string DefaultIcon { get; set; }

        [ConfigurationField(typeof(ItemPickerConfigurationEditor.AllowDuplicatesConfigurationField))]
        public bool AllowDuplicates { get; set; }
    }
}
