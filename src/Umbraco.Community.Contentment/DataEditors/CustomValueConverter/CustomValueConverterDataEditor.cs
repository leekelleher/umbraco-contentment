/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
#if NET472
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Core.Strings;
#else
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class CustomValueConverterDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "CustomValueConverter";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Custom Value Converter";
        internal const string DataEditorViewPath = Constants.Internals.EmptyEditorViewPath;
        internal const string DataEditorIcon = "icon-defrag";

        private readonly IDataTypeService _dataTypeService;
        private readonly PropertyEditorCollection _propertyEditors;
        private readonly PropertyValueConverterCollection _propertyValueConverters;
        private readonly IIOHelper _ioHelper;
        private readonly IShortStringHelper _shortStringHelper;

#if NET472
        public CustomValueConverterDataEditor(
            IDataTypeService dataTypeService,
            PropertyEditorCollection propertyEditors,
            PropertyValueConverterCollection propertyValueConverters,
            IIOHelper ioHelper,
            IShortStringHelper shortStringHelper)
        {
            _dataTypeService = dataTypeService;
            _propertyEditors = propertyEditors;
            _propertyValueConverters = propertyValueConverters;
            _ioHelper = ioHelper;
            _shortStringHelper = shortStringHelper;
        }
#else
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IJsonSerializer _jsonSerializer;

        public CustomValueConverterDataEditor(
            IDataTypeService dataTypeService,
            PropertyEditorCollection propertyEditors,
            PropertyValueConverterCollection propertyValueConverters,
            IIOHelper ioHelper,
            ILocalizedTextService localizedTextService,
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer)
        {
            _dataTypeService = dataTypeService;
            _propertyEditors = propertyEditors;
            _propertyValueConverters = propertyValueConverters;
            _ioHelper = ioHelper;
            _localizedTextService = localizedTextService;
            _shortStringHelper = shortStringHelper;
            _jsonSerializer = jsonSerializer;
        }
#endif

        public string Alias => DataEditorAlias;

        public EditorType Type => EditorType.PropertyValue;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => Constants.Conventions.PropertyGroups.Code;

        public bool IsDeprecated => false;

        public IDictionary<string, object> DefaultConfiguration => default;

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor() => new CustomValueConverterConfigurationEditor(
            _dataTypeService,
            _propertyEditors,
            _propertyValueConverters,
            _shortStringHelper,
            _ioHelper);

        public IDataValueEditor GetValueEditor()
        {
#if NET472
            return new DataValueEditor
#else
            return new DataValueEditor(
                _localizedTextService,
                _shortStringHelper,
                _jsonSerializer)
#endif
            {
                ValueType = ValueTypes.Json,
                View = _ioHelper.ResolveRelativeOrVirtualUrl(DataEditorViewPath)
            };
        }

        public IDataValueEditor GetValueEditor(object configuration)
        {
            if (configuration is Dictionary<string, object> config &&
                config.TryGetValueAs(CustomValueConverterConfigurationEditor.DataType, out GuidUdi udi) == true)
            {
                var dataType = _dataTypeService.GetDataType(udi.Guid);
                if (dataType != null && _propertyEditors.TryGet(dataType.EditorAlias, out var dataEditor) == true)
                {
                    return dataEditor.GetValueEditor();
                }
            }

            return GetValueEditor();
        }
    }
}
