/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue | EditorType.MacroParameter,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Json,
        Group = Core.Constants.PropertyEditors.Groups.Pickers,
        Icon = DataEditorIcon)]
    [Core.Composing.HideFromTypeFinder]
    internal sealed class MacroPickerDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "MacroPicker";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Macro Picker";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "macro-picker.html";
        internal const string DataEditorIcon = Core.Constants.Icons.Macro;

        private readonly IMacroService _macroService;

        public MacroPickerDataEditor(ILogger logger, IMacroService macroService)
            : base(logger)
        {
            _macroService = macroService;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new MacroPickerConfigurationEditor(_macroService);
    }
}
