/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using UmbracoIcons = Umbraco.Core.Constants.Icons;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
#if DEBUG
        EditorType.PropertyValue | EditorType.MacroParameter, // NOTE: IsWorkInProgress [LK]
#else
        EditorType.Nothing,
#endif
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Json,
        Group = Constants.Conventions.PropertyGroups.Pickers,
        Icon = DataEditorIcon)]
    public class MacroPickerDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "MacroPicker";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Macro Picker";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "macro-picker.html";
        internal const string DataEditorIcon = UmbracoIcons.Macro;

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
