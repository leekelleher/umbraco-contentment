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

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new DefaultIconConfigurationField(),
            new ItemPickerTypeConfigurationField(),
            new MaxItemsConfigurationField(),
            new AllowDuplicatesConfigurationField(),
            new EnableMultipleConfigurationField(),
            new DisableSortingConfigurationField(),
        };

        public Dictionary<string, object> DefaultValues => default;

        public Dictionary<string, object> DefaultConfig => new Dictionary<string, object>
        {
            { ItemPickerConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorOverlayViewPath) },
            { ItemPickerConfigurationEditor.OverlayOrderBy, string.Empty },
        };

        public bool HasMultipleValues => true;

        public string View => ItemPickerDataEditor.DataEditorViewPath;
    }
}
