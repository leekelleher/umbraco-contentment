/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
#if NET472
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
#else
#if NET7_0_OR_GREATER
using System;
using Umbraco.Cms.Core.Configuration;
#endif
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class IconPickerConfigurationEditor : ConfigurationEditor
    {
#if NET7_0_OR_GREATER
        public IconPickerConfigurationEditor(IIOHelper ioHelper, IUmbracoVersion umbracoVersion)
#else
        public IconPickerConfigurationEditor(IIOHelper ioHelper)
#endif
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

#if NET7_0_OR_GREATER
            if (umbracoVersion.Version >= new Version(11, 2))
            {
                Fields.Add(new ConfigurationField
                {
                    Key = "hideColors",
                    Name = "Hide colors?",
                    Description = "Select to hide the color options in the icon picker panel.",
                    View = "boolean",
                });
            }
#endif
        }
    }
}
