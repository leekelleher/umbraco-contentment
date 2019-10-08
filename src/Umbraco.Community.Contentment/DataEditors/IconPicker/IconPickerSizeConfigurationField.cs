/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class IconPickerSizeConfigurationField : ConfigurationField
    {
        internal const string Size = "size";
        internal const string Small = "small";
        internal const string Large = "large";

        public IconPickerSizeConfigurationField()
            : this(Large)
        { }

        public IconPickerSizeConfigurationField(string defaultValue)
            : base()
        {
            Key = Size;
            Name = nameof(Size);
            Description = $"Select the size of the icon picker. By default this is set to '{Large}'.<br><br>If '{Small}' is selected, the description and delete button will not be visible.";
            View = IOHelper.ResolveUrl(RadioButtonListDataEditor.DataEditorViewPath);
            Config = new Dictionary<string, object>
            {
                { RadioButtonListConfigurationEditor.Items, new[]
                    {
                        new DataListItem { Name = nameof(Small), Value = Small },
                        new DataListItem { Name = nameof(Large), Value = Large }
                    }
                },
                { RadioButtonListConfigurationEditor.DefaultValue, defaultValue }
            };
        }
    }
}
