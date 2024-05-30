/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Configuration;
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
            //Fields.Add(new ContentmentConfigurationField
            //{
            //    Key = "defaultIcon",
            //    Name = "Default icon",
            //    Description = "Select an icon to be displayed as the default icon, (for when no icon has been selected).",
            //    View = ioHelper.ResolveRelativeOrVirtualUrl(IconPickerDataEditor.DataEditorViewPath),
            //    Config = new Dictionary<string, object>
            //    {
            //        { "defaultIcon", string.Empty },
            //        { "size", "large" },
            //    }
            //});

            //Fields.Add(new ContentmentConfigurationField
            //{
            //    Key = "size",
            //    Name = "Size",
            //    Description = "Select the size of icon picker. The default is 'large'.",
            //    View = ioHelper.ResolveRelativeOrVirtualUrl(ButtonsDataListEditor.DataEditorViewPath),
            //    Config = new Dictionary<string, object>
            //    {
            //        { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
            //            {
            //                new DataListItem { Name = "Small", Value = "small" },
            //                new DataListItem { Name = "Large", Value = "large" }
            //            }
            //        },
            //        { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, "large" },
            //        { "labelStyle", "text" },
            //        { "size", "m" },
            //    }
            //});

            //Fields.Add(new ContentmentConfigurationField
            //{
            //    Key = "hideColors",
            //    Name = "Hide colors?",
            //    Description = "Select to hide the color options in the icon picker panel.",
            //    View = "boolean",
            //});
        }
    }
}
