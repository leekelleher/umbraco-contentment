﻿/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class TogglesDataListEditor : IDataListEditor
    {
        public string Name => "Toggles";

        public string Description => "Select multiple values from a group of toggles.";

        public string Icon => TogglesDataEditor.DataEditorIcon;

        public Dictionary<string, object> DefaultConfig => default;

        public string View => IOHelper.ResolveUrl(TogglesDataEditor.DataEditorViewPath);
    }
}