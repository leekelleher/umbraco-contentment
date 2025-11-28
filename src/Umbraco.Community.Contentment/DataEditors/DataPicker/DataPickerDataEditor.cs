/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class DataPickerDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "DataPicker";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Data Picker";
        internal const string DataEditorIcon = "icon-fa-arrow-pointer";
        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "DataPicker";

        private readonly IShortStringHelper _shortStringHelper;
        private readonly IJsonSerializer _jsonSerializer;

        public DataPickerDataEditor(
            IJsonSerializer jsonSerializer,
            IShortStringHelper shortStringHelper)
        {
            _jsonSerializer = jsonSerializer;
            _shortStringHelper = shortStringHelper;
        }

        public string Alias => DataEditorAlias;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => UmbConstants.PropertyEditors.Groups.Lists;

        public bool IsDeprecated => false;

        public IDictionary<string, object>? DefaultConfiguration => default;

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor() => new ConfigurationEditor();

        public IDataValueEditor GetValueEditor()
        {
            return new DataValueEditor(_shortStringHelper, _jsonSerializer)
            {
                ValueType = ValueTypes.Json,
            };
        }

        public IDataValueEditor GetValueEditor(object? configuration)
        {
            return new DataValueEditor(_shortStringHelper, _jsonSerializer)
            {
                ConfigurationObject = configuration,
                ValueType = ValueTypes.Json,
            };
        }
    }
}
