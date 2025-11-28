/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class ContentBlocksDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "ContentBlocks";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Content Blocks";
        internal const string DataEditorIcon = "icon-fa-server";
        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "ContentBlocks";

        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeConfigurationCache _dataTypeConfigurationCache;
        private readonly Lazy<PropertyEditorCollection> _propertyEditors;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IPropertyValidationService _propertyValidationService;

        public ContentBlocksDataEditor(
            IContentTypeService contentTypeService,
            Lazy<PropertyEditorCollection> propertyEditors,
            IDataTypeConfigurationCache dataTypeConfigurationCache,
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer,
            IPropertyValidationService propertyValidationService)
        {
            _contentTypeService = contentTypeService;
            _dataTypeConfigurationCache = dataTypeConfigurationCache;
            _shortStringHelper = shortStringHelper;
            _jsonSerializer = jsonSerializer;
            _propertyEditors = propertyEditors;
            _propertyValidationService = propertyValidationService;
        }

        public string Alias => DataEditorAlias;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => UmbConstants.PropertyEditors.Groups.RichContent;

        public bool IsDeprecated => false;

        public IDictionary<string, object>? DefaultConfiguration => default;

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor() => new ConfigurationEditor();

        public IDataValueEditor GetValueEditor()
        {
            return new ContentBlocksDataValueEditor(
                _contentTypeService,
                _propertyEditors.Value,
                _dataTypeConfigurationCache,
                _shortStringHelper,
                _jsonSerializer,
                _propertyValidationService)
            {
                ValueType = ValueTypes.Json,
            };
        }

        public IDataValueEditor GetValueEditor(object? configuration)
        {
            return new ContentBlocksDataValueEditor(
                _contentTypeService,
                _propertyEditors.Value,
                _dataTypeConfigurationCache,
                _shortStringHelper,
                _jsonSerializer,
                _propertyValidationService)
            {
                ConfigurationObject = configuration,
                ValueType = ValueTypes.Json,
            };
        }
    }
}
