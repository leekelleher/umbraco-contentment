/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.AspNetCore.Hosting;
#if NET472
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;
#else
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
#endif

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

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IIOHelper _ioHelper;

#if NET472
        public CodeEditorDataEditor(
            IWebHostEnvironment webHostEnvironment,
            IIOHelper ioHelper,
            ILogger logger)
            : base(logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _ioHelper = ioHelper;
        }
#else
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IJsonSerializer _jsonSerializer;

        public CodeEditorDataEditor(
            IDataValueEditorFactory dataValueEditorFactory,
            IWebHostEnvironment webHostEnvironment,
            IIOHelper ioHelper,
            ILocalizedTextService localizedTextService,
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer)
            : base(dataValueEditorFactory)
        {
            _webHostEnvironment = webHostEnvironment;
            _ioHelper = ioHelper;
            _localizedTextService = localizedTextService;
            _shortStringHelper = shortStringHelper;
            _jsonSerializer = jsonSerializer;
        }
#endif

        protected override IConfigurationEditor CreateConfigurationEditor() => new CodeEditorConfigurationEditor(
            _webHostEnvironment,
            _ioHelper);

#if NET472
        protected override IDataValueEditor CreateValueEditor() => new TextOnlyValueEditor(Attribute);
#else
        protected override IDataValueEditor CreateValueEditor() => new TextOnlyValueEditor(
            Attribute,
            _localizedTextService,
            _shortStringHelper,
            _jsonSerializer,
            _ioHelper);
#endif
    }
}
