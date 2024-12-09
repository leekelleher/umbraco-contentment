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
    public sealed class DataListDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "DataList";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Data List";
        internal const string DataEditorIcon = "icon-fa-list-ul";
        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "DataList";

        private readonly IShortStringHelper _shortStringHelper;
        private readonly IJsonSerializer _jsonSerializer;

        public DataListDataEditor(
            IJsonSerializer jsonSerializer,
            IShortStringHelper shortStringHelper)
        {
            _shortStringHelper = shortStringHelper;
            _jsonSerializer = jsonSerializer;
        }

        public string Alias => DataEditorAlias;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => UmbConstants.PropertyEditors.Groups.Lists;

        public bool IsDeprecated => false;

        public IDictionary<string, object> DefaultConfiguration => new Dictionary<string, object>();

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor() => new ConfigurationEditor();

        public IDataValueEditor GetValueEditor()
        {
            return new DataListDataValueEditor(_shortStringHelper, _jsonSerializer)
            {
                ValueType = ValueTypes.Json,
            };
        }

        public IDataValueEditor GetValueEditor(object? configuration)
        {
            return new DataListDataValueEditor(_shortStringHelper, _jsonSerializer)
            {
                ConfigurationObject = configuration,
                ValueType = ValueTypes.Json,
            };
        }
    }
}
