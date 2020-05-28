/* Copyright © 2020 Lee Kelleher.
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
        ValueType = ValueTypes.Integer,
        Group = Core.Constants.PropertyEditors.Groups.Common,
        Icon = DataEditorIcon)]
    internal sealed class NumberInputDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "NumberInput";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Number Input";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "number-input.html";
        internal const string DataEditorIcon = "icon-coin";

        public NumberInputDataEditor(ILogger logger)
            : base(logger)
        { }

        // TODO: [LK:2020-01-02] Add config options for "size: `umb-property-editor-tiny`, `umb-property-editor-small`, `umb-property-editor--limit-width`, or standard full width".
    }
}
