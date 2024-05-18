/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ListItemsConfigurationEditor : ConfigurationEditor
    {
        public ListItemsConfigurationEditor(IIOHelper ioHelper)
            : base()
        {
            Fields.Add(new ConfigurationField
            {
                Key = "hideIcon",
                Name = "Hide icon field?",
                Description = "Select to hide the icon picker.",
                View = "boolean",
            });

            Fields.Add(new ConfigurationField
            {
                Key = "hideDescription",
                Name = "Hide description field?",
                Description = "Select to hide the description text field.",
                View = "boolean",
            });

            Fields.Add(new ConfigurationField
            {
                Key = "confirmRemoval",
                Name = "Confirm removals?",
                Description = "Select to enable a confirmation prompt when removing an item.",
                View = "boolean",
            });

            DefaultConfiguration.Add(MaxItemsConfigurationField.MaxItems, 0);
            Fields.Add(new MaxItemsConfigurationField(ioHelper));
            Fields.Add(new EnableDevModeConfigurationField());
        }
    }
}
