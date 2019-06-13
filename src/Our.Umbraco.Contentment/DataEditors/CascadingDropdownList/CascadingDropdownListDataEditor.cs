/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

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
    public class CascadingDropdownListDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "CascadingDropdownList";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Cascading Dropdown List";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "cascading-dropdown-list.html";
        internal const string DataEditorIcon = "icon-indent";

        public CascadingDropdownListDataEditor(ILogger logger)
            : base(logger)
        { }

        // TODO: [LK:2019-06-13] When we add a ConfigurationEditor class, add the field "apis" + const string
    }
}
