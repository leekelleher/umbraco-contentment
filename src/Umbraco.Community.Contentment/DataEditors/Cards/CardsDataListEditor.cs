/* Copyright © 2019 Lee Kelleher.
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
    public sealed class CardsDataListEditor : IDataListEditor
    {
        public string Name => "Cards";

        public string Description => "Select items to add to a list, displayed as cards.";

        public string Icon => CardsDataEditor.DataEditorIcon;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new DefaultIconConfigurationField(),
            new ShowDescriptionsConfigurationField(),
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

        public string View => IOHelper.ResolveUrl(CardsDataEditor.DataEditorViewPath);
    }
}
