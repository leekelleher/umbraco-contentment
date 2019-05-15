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
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Json,
        Group = "Picker",
        Icon = "icon-science")]
#if DEBUG
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
#endif
    public class UmbracoEntityPickerDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.UmbracoEntityPicker";
        internal const string DataEditorName = "[Contentment] Umbraco Entity Picker";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/umb-entity-picker.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/umb-entity-picker.js";

        public UmbracoEntityPickerDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new UmbracoEntityPickerConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new HideLabelDataValueEditor(Attribute);
    }
}
