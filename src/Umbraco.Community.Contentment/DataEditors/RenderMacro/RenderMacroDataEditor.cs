﻿/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class RenderMacroDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "RenderMacro";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Render Macro";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "render-macro.html";
        internal const string DataEditorIcon = Core.Constants.Icons.Macro;

        public string Alias => DataEditorAlias;

        public EditorType Type => EditorType.PropertyValue;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => Constants.Conventions.PropertyGroups.Display;

        public bool IsDeprecated => false;

        public IDictionary<string, object> DefaultConfiguration => default;

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor() => new RenderMacroConfigurationEditor();

        public IDataValueEditor GetValueEditor()
        {
            return new ReadOnlyDataValueEditor
            {
                ValueType = ValueTypes.Integer,
                View = DataEditorViewPath,
            };
        }

        public IDataValueEditor GetValueEditor(object configuration)
        {
            var hideLabel = false;

            if (configuration is Dictionary<string, object> config && config.ContainsKey(HideLabelConfigurationField.HideLabelAlias) == true)
            {
                hideLabel = config[HideLabelConfigurationField.HideLabelAlias].TryConvertTo<bool>().Result;
            }

            return new ReadOnlyDataValueEditor
            {
                Configuration = configuration,
                HideLabel = hideLabel,
                ValueType = ValueTypes.Integer,
                View = DataEditorViewPath,
            };
        }
    }
}
