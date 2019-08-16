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
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Integer,
        Group = Constants.Conventions.PropertyGroups.Display,
        Icon = DataEditorIcon)]
    public class NotesDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "Notes";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Notes";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "notes.html";
        internal const string DataEditorIcon = "icon-readonly";

        public NotesDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new NotesConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new ReadOnlyDataValueEditor(Attribute);
    }
}
