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
        Group = "Lists",
        Icon = "icon-bulleted-list",
        IsDeprecated = false // NOTE: IsWorkInProgress
        )]
#if DEBUG
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
    [PropertyEditorAsset(ClientDependencyType.Javascript, "~/App_Plugins/Contentment/data-editors/configuration-editor-picker.js")]
#endif
    public class DataListDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.DataList";
        internal const string DataEditorName = "[Contentment] Data List";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/data-list.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/data-list.js";

        public DataListDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new DataListConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new DataListDataValueEditor(Attribute);
    }
}
