/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class TemplatedLabelDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "TemplatedLabel";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Templated Label";
        internal const string DataEditorViewPath = NotesDataEditor.DataEditorViewPath;
        internal const string DataEditorIcon = "icon-fa fa-codepen";

        public string Alias => DataEditorAlias;

        public EditorType Type => EditorType.PropertyValue;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => Constants.Conventions.PropertyGroups.Display;

        public bool IsDeprecated => false;

        public IDictionary<string, object> DefaultConfiguration => default;

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor() => new TemplatedLabelConfigurationEditor();

        public IDataValueEditor GetValueEditor()
        {
            return new DataValueEditor
            {
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

            return new DataValueEditor
            {
                Configuration = configuration,
                HideLabel = hideLabel,
                View = DataEditorViewPath,
            };
        }
    }
}
