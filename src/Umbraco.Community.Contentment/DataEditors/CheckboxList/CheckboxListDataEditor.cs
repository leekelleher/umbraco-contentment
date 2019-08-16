/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
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
        Group = Constants.Conventions.PropertyGroups.Lists,
        Icon = DataEditorIcon)]
    public class CheckboxListDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "CheckboxList";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Checkbox List";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "checkbox-list.html";
        internal const string DataEditorIcon = "icon-bulleted-list";

        public CheckboxListDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new CheckboxListConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new HideLabelDataValueEditor(Attribute);
    }
}
