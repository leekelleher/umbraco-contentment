/* Copyright © 2019 Lee Kelleher.
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
using Umbraco.Cms.Core.PropertyEditors;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue | EditorType.MacroParameter,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.String,
        Group = UmbConstants.PropertyEditors.Groups.Pickers,
        Icon = DataEditorIcon)]
    public sealed class IconPickerDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "IconPicker";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Icon Picker";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "icon-picker.html";
        internal const string DataEditorIcon = "icon-fa fa-circle-o";

        private readonly IIOHelper _ioHelper;

#if NET472
        public IconPickerDataEditor(IIOHelper ioHelper, ILogger logger)
            : base(logger)
#else
        public IconPickerDataEditor(IIOHelper ioHelper, IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
#endif
        {
            _ioHelper = ioHelper;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new IconPickerConfigurationEditor(_ioHelper);
    }
}
