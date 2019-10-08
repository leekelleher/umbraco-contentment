/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class RadioButtonListDataListEditor : IDataListEditor
    {
        public string Name => "Radio Button List";

        public string Description => "Select a single value from a list of radio buttons";

        public string Icon => RadioButtonListDataEditor.DataEditorIcon;

        public Dictionary<string, object> DefaultConfig => default;

        public string View => IOHelper.ResolveUrl(RadioButtonListDataEditor.DataEditorViewPath);

        [ConfigurationField(typeof(ShowDescriptionsConfigurationField))]
        public bool ShowDescriptions { get; set; }
    }
}
