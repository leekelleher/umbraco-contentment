/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using ClientDependency.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Text,
        Group = "Code",
        Icon = "icon-code",
        IsDeprecated = false // NOTE: IsWorkInProgress [LK]
        )]
#if DEBUG
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
#endif
    public class CodeEditorDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.CodeEditor";
        internal const string DataEditorName = "[Contentment] Code Editor";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/code-editor.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/code-editor.js";

        public CodeEditorDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new CodeEditorConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new HideLabelDataValueEditor(Attribute);
    }
}
