/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class ToggleGroupDataListEditor : IDataListEditor
    {
        public string Name => "Toggle Group";

        public string Description => "Select multiple values from a group of toggles.";

        public string Icon => ToggleGroupDataEditor.DataEditorIcon;

        public Dictionary<string, object> DefaultConfig => default;

        public string View => IOHelper.ResolveUrl(ToggleGroupDataEditor.DataEditorViewPath);
    }
}
