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
        Icon = "icon-item-arrangement"
#if !DEBUG
        ,IsDeprecated = true // NOTE: IsWorkInProgress [LK]
#endif
        )]
#if DEBUG
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
#endif
    public class DataTableDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.DataTable";
        internal const string DataEditorName = "[Contentment] Data Table";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/data-table.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/data-table.js";

        public DataTableDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new DataTableConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new HideLabelDataValueEditor(Attribute);
    }
}
