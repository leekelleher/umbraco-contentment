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
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Json,
        Group = Core.Constants.PropertyEditors.Groups.Pickers,
#if DEBUG
        Icon = "icon-block color-red"
#else
        Icon = DataEditorIcon
#endif
        )]
    internal sealed class ItemPickerDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "ItemPicker";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Item Picker";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "item-picker.html";
        internal const string DataEditorOverlayViewPath = Constants.Internals.EditorsPathRoot + "item-picker.overlay.html";
        internal const string DataEditorIcon = "icon-hand-pointer";

        public ItemPickerDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new ItemPickerConfigurationEditor();
    }
}
