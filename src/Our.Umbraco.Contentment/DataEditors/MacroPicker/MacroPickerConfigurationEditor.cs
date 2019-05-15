/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Models.ContentEditing;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class MacroPickerConfigurationEditor : ConfigurationEditor
    {
        public MacroPickerConfigurationEditor()
            : base()
        {
            Fields.Add(
                "allowedMacros",
                "Allowed Macros",
                "Restrict the macros that can be picked.",
                IOHelper.ResolveUrl(UmbracoEntityPickerDataEditor.DataEditorViewPath),
                new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationEditors.EntityType, nameof(UmbracoEntityTypes.Macro) },
                    { "semisupportedTypes", new[]{ nameof(UmbracoEntityTypes.Macro) } }
                });
            Fields.AddMaxItems();
            Fields.AddDisableSorting();
            Fields.AddHideLabel();
        }
    }
}
