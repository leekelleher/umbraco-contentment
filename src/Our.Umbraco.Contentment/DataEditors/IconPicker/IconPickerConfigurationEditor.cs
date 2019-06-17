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
            Fields.Add(new DefaultIconConfigurationField());
            Fields.AddHideLabel();
        }

        internal class DefaultIconConfigurationField : ConfigurationField
        {
            public const string DefaultIcon = "defaultIcon";

            public DefaultIconConfigurationField()
                : base()
            {
                Key = DefaultIcon;
                Name = "Default icon";
                Description = "Select an icon to be displayed as the default icon, (for when no icon has been selected).";
                View = IOHelper.ResolveUrl(IconPickerDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    // TODO: [LK:2019-06-17] If/when PR #5618 is merged accepted, we can use `Icons.DefaultIcon`.
                    // https://github.com/umbraco/Umbraco-CMS/pull/5618
                    { DefaultIcon, "icon-document" }
                };
            }
        }
    }
}
