/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Integer,
        Group = Core.Constants.PropertyEditors.Groups.Common,
        Icon = DataEditorIcon)]
    [Core.Composing.HideFromTypeFinder]
    internal sealed class CheckboxDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "Checkbox";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Checkbox";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "checkbox.html";
        internal const string DataEditorIcon = "icon-checkbox";

        public CheckboxDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new CheckboxConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new CheckboxDataValueEditor(Attribute);
    }
}
