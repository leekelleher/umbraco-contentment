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
#if DEBUG
        EditorType.PropertyValue, // NOTE: IsWorkInProgress [LK]
#else
        EditorType.Nothing,
#endif
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Json,
        Group = Constants.Conventions.PropertyGroups.Pickers,
        Icon = DataEditorIcon)]
    public class ConfigurationEditorDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "ConfigurationEditor";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Configuration Editor";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "configuration-editor.html";
        internal const string DataEditorOverlayViewPath = Constants.Internals.EditorsPathRoot + "configuration-editor.overlay.html";
        internal const string DataEditorIcon = UmbracoIcons.Macro;

        public ConfigurationEditorDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new ConfigurationEditorConfigurationEditor();
    }
}
