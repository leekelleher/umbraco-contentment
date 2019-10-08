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
    internal sealed class TogglesDataListEditor : IDataListEditor
    {
        public string Name => "Toggles";

        public string Description => "Select multiple values from a group of toggles.";

        public string Icon => TogglesDataEditor.DataEditorIcon;

        public Dictionary<string, object> DefaultConfig => default;

        public string View => IOHelper.ResolveUrl(TogglesDataEditor.DataEditorViewPath);

        [ConfigurationField(typeof(ShowDescriptionsConfigurationField))]
        public bool ShowDescriptions { get; set; }
    }
}
