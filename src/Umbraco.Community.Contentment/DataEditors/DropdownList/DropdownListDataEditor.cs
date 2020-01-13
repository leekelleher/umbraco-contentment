/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.String,
        Group = Core.Constants.PropertyEditors.Groups.Lists,
        Icon = DataEditorIcon)]
    [Core.Composing.HideFromTypeFinder]
    internal sealed class DropdownListDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "DropdownList";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Dropdown List";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "dropdown-list.html";
        internal const string DataEditorIcon = "icon-indent";

        public DropdownListDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new DropdownListConfigurationEditor();
    }
}
