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
        public string Name => "Radio Button List";

        public string Description => "Select a single value from a list of radio buttons";

        public string Icon => RadioButtonListDataEditor.DataEditorIcon;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ShowDescriptionsConfigurationField(),
            new ShowIconsConfigurationField(),
        };

        public Dictionary<string, object> DefaultValues => default;

        public Dictionary<string, object> DefaultConfig => default;

        public bool HasMultipleValues => false;

        public string View => RadioButtonListDataEditor.DataEditorViewPath;
    }
}
