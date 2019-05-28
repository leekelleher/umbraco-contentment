/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class IconPickerConfigurationEditor : ConfigurationEditor
    {
        public IconPickerConfigurationEditor()
            : base()
        {
            Fields.Add(
                "defaultIcon",
                "Default icon",
                "Select an icon to be displayed as the default icon, (for when no icon has been selected).",
                IOHelper.ResolveUrl(IconPickerDataEditor.DataEditorViewPath),
                new Dictionary<string, object> {
                    { "defaultIcon", "icon-document" }
                });
            Fields.AddHideLabel();
        }
    }
}
