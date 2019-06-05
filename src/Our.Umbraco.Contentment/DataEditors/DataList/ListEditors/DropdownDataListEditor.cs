/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class DropdownDataListEditor : IDataListEditor
    {
        public string Name => "Dropdown List";

        public string Description => "Select a single value from a dropdown select list.";

        public string Icon => DropdownListDataEditor.DataEditorIcon;

        public string View => IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath);

        [ConfigurationField(typeof(DropdownListConfigurationEditor.AllowEmptyConfigurationField))]
        public bool AllowEmpty { get; set; }
    }
}
