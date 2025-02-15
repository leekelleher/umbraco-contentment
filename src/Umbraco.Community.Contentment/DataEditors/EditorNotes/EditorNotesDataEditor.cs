/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class EditorNotesDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "EditorNotes";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Editor Notes";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "editor-notes.html";
        internal const string DataEditorIcon = "icon-alert-alt";
        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "EditorNotes";

        private readonly IShortStringHelper _shortStringHelper;
        private readonly IJsonSerializer _jsonSerializer;

        public EditorNotesDataEditor(
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer)
        {
            _shortStringHelper = shortStringHelper;
            _jsonSerializer = jsonSerializer;
        }

        public string Alias => DataEditorAlias;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => Constants.Conventions.PropertyGroups.Display;

        public bool IsDeprecated => false;

        public IDictionary<string, object>? DefaultConfiguration => default;

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor() => new ConfigurationEditor();

        public IDataValueEditor GetValueEditor()
        {
            return new ReadOnlyDataValueEditor(
                _shortStringHelper,
                _jsonSerializer)
            {
                ValueType = ValueTypes.Integer,
                //View = _ioHelper.ResolveRelativeOrVirtualUrl(DataEditorViewPath)
            };
        }

        public IDataValueEditor GetValueEditor(object? configuration)
        {
            var hideLabel = false;

            if (configuration is Dictionary<string, object> config && config.TryGetValue(HideLabelConfigurationField.HideLabelAlias, out var obj) == true)
            {
                hideLabel = obj.TryConvertTo<bool>().Result;
            }

            return new ReadOnlyDataValueEditor(
                _shortStringHelper,
                _jsonSerializer)
            {
                ConfigurationObject = configuration,
                //HideLabel = hideLabel,
                ValueType = ValueTypes.Integer,
                //View = _ioHelper.ResolveRelativeOrVirtualUrl(DataEditorViewPath)
            };
        }
    }
}
