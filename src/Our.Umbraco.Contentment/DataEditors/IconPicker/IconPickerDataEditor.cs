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
        EditorType.PropertyValue | EditorType.MacroParameter,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.String,
        Group = "Picker",
        Icon = "icon-icon",
        IsDeprecated = true // NOTE: IsWorkInProgress [LK]
        )]
#if DEBUG
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
#endif
    public class IconPickerDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.IconPicker";
        internal const string DataEditorName = "[Contentment] Icon Picker";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/icon-picker.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/icon-picker.js";

        public IconPickerDataEditor(ILogger logger)
            : base(logger)
        { }
    }
}
