/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class DropdownListDataListEditor : IDataListEditor
    {
        public string Name => "Dropdown List";

        public string Description => "Select a single value from a dropdown select list.";

        public string Icon => DropdownListDataEditor.DataEditorIcon;

        public Dictionary<string, object> DefaultConfig => default;

        public string View => IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath);

        [ConfigurationField(typeof(AllowEmptyConfigurationField))]
        public bool AllowEmpty { get; set; }
    }
}
