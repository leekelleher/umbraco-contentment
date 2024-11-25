/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Obsolete("To be removed in Contentment 7.0")]
    internal sealed class RenderMacroConfigurationEditor : ConfigurationEditor
    {
        internal const string Macro = "macro";

        public RenderMacroConfigurationEditor(IIOHelper ioHelper)
            : base()
        {
            //Fields.Add(new ContentmentConfigurationField
            //{
            //    Key = Macro,
            //    Name = nameof(Macro),
            //    Description = "Select and configure the macro to be displayed.",
            //    View = ioHelper.ResolveRelativeOrVirtualUrl(MacroPickerDataEditor.DataEditorViewPath),
            //    Config = new Dictionary<string, object>
            //    {
            //        { MaxItemsConfigurationField.MaxItems, 1 }
            //    }
            //});

            //Fields.Add(new HideLabelConfigurationField());
            //Fields.Add(new HidePropertyGroupConfigurationField());
        }
    }
}
