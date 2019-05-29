/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using ClientDependency.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.PropertyEditors;
using UmbracoIcons = Umbraco.Core.Constants.Icons;

namespace Our.Umbraco.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue | EditorType.MacroParameter,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Json,
        Group = "Picker",
        Icon = UmbracoIcons.Macro)]
#if DEBUG
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
#endif
    public class MacroPickerDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.MacroPicker";
        internal const string DataEditorName = "[Contentment] Macro Picker";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/macro-picker.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/macro-picker.js";

        private readonly IMacroService _macroService;

        public MacroPickerDataEditor(ILogger logger, IMacroService macroService)
            : base(logger)
        {
            _macroService = macroService;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new MacroPickerConfigurationEditor(_macroService);

        protected override IDataValueEditor CreateValueEditor() => new HideLabelDataValueEditor(Attribute);
    }
}
