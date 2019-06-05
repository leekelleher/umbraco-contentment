/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using ClientDependency.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;
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
#if DEBUG
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorOverlayJsPath)]
#endif
    public class ConfigurationEditorDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.ConfigurationEditor";
        internal const string DataEditorName = "[Contentment] Configuration Editor";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/configuration-editor.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/configuration-editor.js";
        internal const string DataEditorOverlayViewPath = "~/App_Plugins/Contentment/data-editors/configuration-editor.overlay.html";
        internal const string DataEditorOverlayJsPath = "~/App_Plugins/Contentment/data-editors/configuration-editor.overlay.js";
        internal const string DataEditorIcon = UmbracoIcons.Macro;

        public ConfigurationEditorDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new ConfigurationEditorConfigurationEditor();
    }
}
