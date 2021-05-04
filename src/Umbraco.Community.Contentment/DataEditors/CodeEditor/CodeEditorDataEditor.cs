/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Text,
        Group = Constants.Conventions.PropertyGroups.Code,
        Icon = DataEditorIcon)]
    internal sealed class CodeEditorDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "CodeEditor";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Code Editor";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "code-editor.html";
        internal const string DataEditorIcon = "icon-fa fa-code";

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IIOHelper _ioHelper;
        private readonly IDataTypeService _dataTypeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IJsonSerializer _jsonSerializer;

        public CodeEditorDataEditor(
            IHostingEnvironment hostingEnvironment,
            IIOHelper ioHelper,
            ILoggerFactory loggerFactory,
            IDataTypeService dataTypeService,
            ILocalizationService localizationService,
            ILocalizedTextService localizedTextService,
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer,
            EditorType type = EditorType.PropertyValue)
            : base(loggerFactory, dataTypeService, localizationService, localizedTextService, shortStringHelper, jsonSerializer, type)
        {
            _hostingEnvironment = hostingEnvironment;
            _ioHelper = ioHelper;
            _dataTypeService = dataTypeService;
            _localizationService = localizationService;
            _localizedTextService = localizedTextService;
            _shortStringHelper = shortStringHelper;
            _jsonSerializer = jsonSerializer;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new CodeEditorConfigurationEditor(
            _hostingEnvironment,
            _ioHelper);

        protected override IDataValueEditor CreateValueEditor() => new TextOnlyValueEditor(
            _dataTypeService,
            _localizationService,
            Attribute,
            _localizedTextService,
            _shortStringHelper,
            _jsonSerializer);
    }
}
