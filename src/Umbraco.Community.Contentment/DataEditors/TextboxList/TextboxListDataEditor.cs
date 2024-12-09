/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class TextboxListDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "TextboxList";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Textbox List";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "textbox-list.html";
        internal const string DataEditorIcon = "icon-thumbnail-list";
        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "TextboxList";

        private readonly IShortStringHelper _shortStringHelper;
        private readonly IJsonSerializer _jsonSerializer;

        public string Alias => DataEditorAlias;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => UmbConstants.PropertyEditors.Groups.Common;

        public bool IsDeprecated => false;

        public IDictionary<string, object>? DefaultConfiguration => default;

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public TextboxListDataEditor(
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer)
        {
            _shortStringHelper = shortStringHelper;
            _jsonSerializer = jsonSerializer;
        }

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
