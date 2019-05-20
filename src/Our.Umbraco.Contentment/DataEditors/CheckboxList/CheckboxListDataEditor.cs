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
        ValueType = ValueTypes.Json, // TODO: JSON or String (CSV)?
        Group = "Lists",
        Icon = "icon-bulleted-list",
        IsDeprecated = true // NOTE: IsWorkInProgress
        )]
#if DEBUG
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
#endif
    public class CheckboxListDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.CheckboxList";
        internal const string DataEditorName = "[Contentment] Checkbox List";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/checkbox-list.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/checkbox-list.js";

        public CheckboxListDataEditor(ILogger logger)
            : base(logger)
        { }
    }
}
