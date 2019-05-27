/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.IO;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class CheckboxListDataListEditor : IDataListEditor
    {
        public string Name => "Checkbox List";

        public string Description => "Select multiple values from a list of checkboxes.";

        public string Icon => "icon-bulleted-list";

        public string View => IOHelper.ResolveUrl(CheckboxListDataEditor.DataEditorViewPath);
    }
}
