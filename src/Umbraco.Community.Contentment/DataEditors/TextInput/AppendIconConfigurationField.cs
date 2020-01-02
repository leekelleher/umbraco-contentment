/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class AppendIconConfigurationField : ConfigurationField
    {
        public const string AppendIcon = "append";

        public AppendIconConfigurationField()
        {
            Name = "Append Icon";
            Key = AppendIcon;
            Description = "[Add friendly description]";
            View = IOHelper.ResolveUrl(IconPickerDataEditor.DataEditorViewPath);
            Config = new Dictionary<string, object>
            {
                { DefaultIconConfigurationField.DefaultIcon, string.Empty },
                { IconPickerSizeConfigurationField.Size, IconPickerSizeConfigurationField.Large }
            };
        }
    }
}
