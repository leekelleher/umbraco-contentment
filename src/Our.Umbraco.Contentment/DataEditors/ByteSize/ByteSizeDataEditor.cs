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
#if DEBUG
        EditorType.PropertyValue, // NOTE: IsWorkInProgress [LK]
#else
        EditorType.Nothing,
#endif
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Bigint,
        Group = Constants.Conventions.PropertyGroups.Display,
        Icon = DataEditorIcon)]
#if DEBUG
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
#endif
    public class ByteSizeDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.ByteSize";
        internal const string DataEditorName = "[Contentment] Byte Size";
        internal const string DataEditorViewPath = "readonlyvalue";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/byte-size.js";
        internal const string DataEditorIcon = "icon-binarycode";

        public ByteSizeDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new ByteSizeConfigurationEditor();
    }
}
