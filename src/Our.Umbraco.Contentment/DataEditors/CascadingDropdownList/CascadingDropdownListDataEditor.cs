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
#if DEBUG
        EditorType.PropertyValue, // NOTE: IsWorkInProgress [LK]
#else
        EditorType.Nothing,
#endif
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Json,
        Group =  Constants.Conventions.PropertyGroups.Lists,
        Icon = DataEditorIcon)]
#if DEBUG
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
#endif
    public class CascadingDropdownListDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.CascadingDropdownList";
        internal const string DataEditorName = "[Contentment] Cascading Dropdown List";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/cascading-dropdown-list.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/cascading-dropdown-list.js";
        internal const string DataEditorIcon = "icon-indent";

        public CascadingDropdownListDataEditor(ILogger logger)
            : base(logger)
        { }
    }
}
