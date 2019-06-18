/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using UmbracoIcons = Umbraco.Core.Constants.Icons;

namespace Our.Umbraco.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Integer,
        Group = Constants.Conventions.PropertyGroups.Display,
        Icon = DataEditorIcon)]
    public class RenderMacroDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "RenderMacro";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Render Macro";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "render-macro.html";
        internal const string DataEditorIcon = UmbracoIcons.Macro;

        public RenderMacroDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new RenderMacroConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new ReadOnlyDataValueEditor(Attribute);
    }
}
