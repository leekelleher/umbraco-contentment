/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class RadioButtonListDataListEditor : IDataListEditor
    {
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "radio-button-list.html";

        public string Name => "Radio Button List";

        public string Description => "Select a single value from a list of radio buttons";

        public string Icon => "icon-target";

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ShowDescriptionsConfigurationField(),
            new ShowIconsConfigurationField(),
        };

        public Dictionary<string, object> DefaultValues => default;

        public Dictionary<string, object> DefaultConfig => default;

        public bool HasMultipleValues(Dictionary<string, object> config) => false;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public string View => DataEditorViewPath;
    }
}
