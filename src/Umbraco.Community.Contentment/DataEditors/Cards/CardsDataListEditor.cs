/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [HideFromTypeFinder]
    internal sealed class CardsDataListEditor : IDataListEditor
    {
        public string Name => "Cards";

        public string Description => "Select items to add to a list, displayed as cards.";

        public string Icon => CardsDataEditor.DataEditorIcon;

        public Dictionary<string, object> DefaultConfig => new Dictionary<string, object>
        {
            { ItemPickerConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorOverlayViewPath) },
            { ItemPickerConfigurationEditor.OverlayOrderBy, string.Empty },
        };

        public string View => IOHelper.ResolveUrl(CardsDataEditor.DataEditorViewPath);

        [ConfigurationField(typeof(DefaultIconConfigurationField))]
        public string DefaultIcon { get; set; }

        [ConfigurationField(typeof(ShowDescriptionsConfigurationField))]
        public bool ShowDescriptions { get; set; }

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
