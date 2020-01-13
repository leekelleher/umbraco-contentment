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
        ValueType = ValueTypes.Json,
        Group = Core.Constants.PropertyEditors.Groups.Lists,
        Icon = DataEditorIcon)]
    public sealed class DataListDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "DataList";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Data List";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "_empty.html";
        internal const string DataEditorIcon = "icon-bulleted-list";

        private readonly ConfigurationEditorUtility _utility;

        public DataListDataEditor(ILogger logger, ConfigurationEditorUtility utility)
            : base(logger)
        {
            _utility = utility;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new DataListConfigurationEditor(_utility);

        protected override IDataValueEditor CreateValueEditor() => new DataListDataValueEditor(Attribute, _utility);
    }
}
