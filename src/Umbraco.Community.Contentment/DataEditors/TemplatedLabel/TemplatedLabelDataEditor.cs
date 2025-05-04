/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(DataEditorAlias, ValueType = ValueTypes.Json)]
    public sealed class TemplatedLabelDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "TemplatedLabel";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Templated Label";
        internal const string DataEditorViewPath = NotesDataEditor.DataEditorViewPath;
        internal const string DataEditorIcon = "icon-fa-codepen";
        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "TemplatedLabel";

        private readonly IIOHelper _ioHelper;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IJsonSerializer _jsonSerializer;

        public TemplatedLabelDataEditor(
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer,
            IIOHelper ioHelper)
        {
            _shortStringHelper = shortStringHelper;
            _jsonSerializer = jsonSerializer;
            _ioHelper = ioHelper;
        }

        public string Alias => DataEditorAlias;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => Constants.Conventions.PropertyGroups.Display;

        public bool IsDeprecated => false;

        public IDictionary<string, object>? DefaultConfiguration => default;

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        // NOTE: Reuses Umbraco's `LabelConfigurationEditor`. [LK]
        public IConfigurationEditor GetConfigurationEditor() => new LabelConfigurationEditor(_ioHelper);

        public IDataValueEditor GetValueEditor()
        {
            return new DataValueEditor(_shortStringHelper, _jsonSerializer) { };
        }

        public IDataValueEditor GetValueEditor(object? configuration)
        {
            var valueType = ValueTypes.String;

            if (configuration is Dictionary<string, object> config &&
                config.TryGetValueAs(UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType, out string? str) == true &&
                string.IsNullOrWhiteSpace(str) == false &&
                ValueTypes.IsValue(str) == true)
            {
                valueType = str;
            }

            return new DataValueEditor(_shortStringHelper, _jsonSerializer)
            {
                ConfigurationObject = configuration,
                ValueType = valueType,
            };
        }
    }
}
