/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Text,
        Group = Constants.Conventions.PropertyGroups.Code,
        Icon = DataEditorIcon)]
    internal sealed class CodeEditorDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "CodeEditor";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Code Editor";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "code-editor.html";
        internal const string DataEditorIcon = "icon-fa fa-code";

        public CodeEditorDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new CodeEditorConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new TextOnlyValueEditor(Attribute);
    }
}
