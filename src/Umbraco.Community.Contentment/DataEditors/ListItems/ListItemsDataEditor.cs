/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Json,
        Group = UmbConstants.PropertyEditors.Groups.Lists,
        Icon = DataEditorIcon)]
    public sealed class ListItemsDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "ListItems";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "List Items";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "data-list.editor.html";
        internal const string DataEditorIcon = "icon-fa fa-th-list";

        private readonly IIOHelper _ioHelper;

        public ListItemsDataEditor(IIOHelper ioHelper, IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new ListItemsConfigurationEditor(_ioHelper);

        protected override IDataValueEditor CreateValueEditor() => DataValueEditorFactory.Create<JsonArrayDataValueEditor>(Attribute!);
    }
}
