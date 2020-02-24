/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class MacroPickerDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "MacroPicker";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Macro Picker";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "macro-picker.html";
        internal const string DataEditorIcon = Core.Constants.Icons.Macro;
    }
}
