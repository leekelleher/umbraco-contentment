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
        Group = Core.Constants.PropertyEditors.Groups.Common,
#if DEBUG
        Icon = DataEditorIcon + " color-red"
#else
        Icon = DataEditorIcon
#endif
        )]
    public class DimensionsDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "Dimensions";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Dimensions";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "dimensions.html";
        internal const string DataEditorIcon = "icon-geometry";

        public DimensionsDataEditor(ILogger logger)
            : base(logger)
        { }
    }
}
