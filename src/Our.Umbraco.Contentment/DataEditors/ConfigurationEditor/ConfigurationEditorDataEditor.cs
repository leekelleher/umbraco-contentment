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
        ValueType = ValueTypes.Json,
        Group = "Picker",
        Icon = "icon-settings-alt",
        IsDeprecated = true // NOTE: IsWorkInProgress
        )]
#if DEBUG
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
#endif
    public class ConfigurationEditorDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.ConfigurationEditor";
        internal const string DataEditorName = "[Contentment] Configuration Editor";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/configuration-editor.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/configuration-editor.js";

        public ConfigurationEditorDataEditor(ILogger logger)
            : base(logger)
        { }
    }
}
