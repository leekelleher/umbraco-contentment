/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue | EditorType.MacroParameter,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.String,
        Group = Core.Constants.PropertyEditors.Groups.Pickers,
#if DEBUG
        Icon = DataEditorIcon + " color-red"
#else
        Icon = DataEditorIcon
#endif
        )]
    public class SocialPickerDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "SocialPicker";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Social Picker";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "social-picker.html";
        internal const string DataEditorIcon = "icon-molecular-network";

        public SocialPickerDataEditor(ILogger logger)
            : base(logger)
        { }
    }
}
