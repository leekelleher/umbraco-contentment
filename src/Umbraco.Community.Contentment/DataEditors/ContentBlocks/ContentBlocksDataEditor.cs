/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class ContentBlocksDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "ContentBlocks";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Content Blocks";
        internal const string DataEditorViewPath = Constants.Internals.EmptyEditorViewPath;
        internal const string DataEditorOverlayViewPath = Constants.Internals.EditorsPathRoot + "content-blocks.overlay.html";
        internal const string DataEditorIcon = "icon-fa fa-server";

        private readonly IContentService _contentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeConfigurationCache _dataTypeConfigurationCache;
        private readonly Lazy<PropertyEditorCollection> _propertyEditors;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly ConfigurationEditorUtility _utility;
        private readonly IIOHelper _ioHelper;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IPropertyValidationService _propertyValidationService;

        public ContentBlocksDataEditor(
            IContentService contentService,
            IContentTypeService contentTypeService,
            Lazy<PropertyEditorCollection> propertyEditors,
            IDataTypeConfigurationCache dataTypeConfigurationCache,
            ILocalizedTextService localizedTextService,
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer,
            ConfigurationEditorUtility utility,
            IIOHelper ioHelper,
            IPropertyValidationService propertyValidationService)
        {
            _contentService = contentService;
            _contentTypeService = contentTypeService;
            _dataTypeConfigurationCache = dataTypeConfigurationCache;
            _localizedTextService = localizedTextService;
            _shortStringHelper = shortStringHelper;
            _jsonSerializer = jsonSerializer;
            _propertyEditors = propertyEditors;
            _utility = utility;
            _ioHelper = ioHelper;
            _propertyValidationService = propertyValidationService;
        }

        public string Alias => DataEditorAlias;

        public EditorType Type => EditorType.PropertyValue;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => UmbConstants.PropertyEditors.Groups.RichContent;

        public bool IsDeprecated => false;

        public IDictionary<string, object>? DefaultConfiguration => default;

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor() => new ContentBlocksConfigurationEditor(_contentService, _contentTypeService, _utility, _ioHelper);

        public IDataValueEditor GetValueEditor()
        {
            return new ContentBlocksDataValueEditor(
                _contentTypeService,
                _propertyEditors.Value,
                _dataTypeConfigurationCache,
                _localizedTextService,
                _shortStringHelper,
                _jsonSerializer,
                _propertyValidationService)
            {
                ValueType = ValueTypes.Json,
                View = _ioHelper.ResolveRelativeOrVirtualUrl(DataEditorViewPath),
            };
        }

        public IDataValueEditor GetValueEditor(object? configuration)
        {
            var view = default(string);

            if (configuration is Dictionary<string, object> config)
            {
                if (config.TryGetValue(ContentBlocksConfigurationEditor.DisplayMode, out var tmp1) == true)
                {
                    var displayMode = default(IContentBlocksDisplayMode);

                    if (tmp1 is string str1 && str1?.InvariantStartsWith(Constants.Internals.EditorsPathRoot) == true)
                    {
                        displayMode = _utility.FindConfigurationEditor<IContentBlocksDisplayMode>(x => str1.InvariantEquals(x.View) == true);
                    }
                    else if (tmp1 is JArray array1 &&
                        array1.Count > 0 &&
                        array1[0] is JObject item1 &&
                        item1.Value<string>("key") is string key)
                    {
                        displayMode = _utility.GetConfigurationEditor<IContentBlocksDisplayMode>(key);
                    }

                    if (displayMode != null)
                    {
                        view = displayMode.View;
                    }
                }
            }

            return new ContentBlocksDataValueEditor(
                _contentTypeService,
                _propertyEditors.Value,
                _dataTypeConfigurationCache,
                _localizedTextService,
                _shortStringHelper,
                _jsonSerializer,
                _propertyValidationService)
            {
                Configuration = configuration,
                ValueType = ValueTypes.Json,
                View = _ioHelper.ResolveRelativeOrVirtualUrl(view ?? DataEditorViewPath),
            };
        }
    }
}
