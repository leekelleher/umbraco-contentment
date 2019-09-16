/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public class RenderMacroConfigurationEditor : ConfigurationEditor
    {
        public const string Macro = "macro";

        public RenderMacroConfigurationEditor()
            : base()
        {
            Fields.Add(
                Macro,
                nameof(Macro),
                "Select and configure the macro to be displayed.",
                IOHelper.ResolveUrl(MacroPickerDataEditor.DataEditorViewPath),
                new Dictionary<string, object>
                {
                    { MaxItemsConfigurationField.MaxItems, 1 }
                });

            Fields.Add(new HideLabelConfigurationField());
        }
    }
}
