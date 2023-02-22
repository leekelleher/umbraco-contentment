/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

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

#if NET472
        public ListItemsDataEditor(IIOHelper ioHelper, ILogger logger)
            : base(logger)
        {
            _ioHelper = ioHelper;
        }
#else
        public ListItemsDataEditor(IIOHelper ioHelper, IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
        }
#endif

        protected override IConfigurationEditor CreateConfigurationEditor() => new ListItemsConfigurationEditor(_ioHelper);

#if NET472
        protected override IDataValueEditor CreateValueEditor() => new JsonArrayDataValueEditor(Attribute);
#else
        protected override IDataValueEditor CreateValueEditor() => DataValueEditorFactory.Create<JsonArrayDataValueEditor>(Attribute);
#endif
    }
}
