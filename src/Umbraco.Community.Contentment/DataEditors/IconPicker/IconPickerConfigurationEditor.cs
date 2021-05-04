/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class IconPickerConfigurationEditor : ConfigurationEditor
    {
        public IconPickerConfigurationEditor(IIOHelper ioHelper)
            : base()
        {
            Fields.Add(new ConfigurationField
            {
                Key = "defaultIcon",
                Name = "Default icon",
                Description = "Select an icon to be displayed as the default icon, (for when no icon has been selected).",
                View = ioHelper.ResolveRelativeOrVirtualUrl(IconPickerDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { "defaultIcon", string.Empty },
                    { "size", "large" },
                }
            });
        }
    }
}
