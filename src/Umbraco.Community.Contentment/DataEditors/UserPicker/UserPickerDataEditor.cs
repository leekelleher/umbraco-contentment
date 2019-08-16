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
        Group = "People",
        Icon = DataEditorIcon)]
    public class UserPickerDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "UserPicker";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "User Picker";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "user-picker.html";
        internal const string DataEditorIcon = "icon-user-females-alt";

        public UserPickerDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new UserPickerConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new HideLabelDataValueEditor(Attribute);
    }
}
