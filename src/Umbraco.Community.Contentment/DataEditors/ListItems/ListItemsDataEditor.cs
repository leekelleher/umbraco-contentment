/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        ValueType = ValueTypes.Json,
        ValueEditorIsReusable = true)]
    public sealed class ListItemsDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "ListItems";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "List Items";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "data-list.editor.html";
        internal const string DataEditorIcon = "icon-fa-table-list";
        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "ListItems";

        public ListItemsDataEditor(IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
        { }

        protected override IDataValueEditor CreateValueEditor() => DataValueEditorFactory.Create<JsonArrayDataValueEditor>(Attribute!);
    }
}
