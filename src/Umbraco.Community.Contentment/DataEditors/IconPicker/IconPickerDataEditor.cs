/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue | EditorType.MacroParameter,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.String,
        Group = Cms.Core.Constants.PropertyEditors.Groups.Pickers,
        Icon = DataEditorIcon)]
    public sealed class IconPickerDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "IconPicker";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Icon Picker";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "icon-picker.html";
        internal const string DataEditorIcon = "icon-fa fa-circle-o";
        private readonly IIOHelper _ioHelper;

        public IconPickerDataEditor(
            IIOHelper ioHelper,
            IDataValueEditorFactory dataValueEditorFactory,
            EditorType type = EditorType.PropertyValue)
            : base(dataValueEditorFactory, type)
        {
            _ioHelper = ioHelper;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new IconPickerConfigurationEditor(_ioHelper);
    }
}
