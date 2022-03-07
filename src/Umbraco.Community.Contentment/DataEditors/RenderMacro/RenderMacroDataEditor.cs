/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
#if NET472
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class RenderMacroDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "RenderMacro";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Render Macro";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "render-macro.html";
        internal const string DataEditorIcon = UmbConstants.Icons.Macro;

        private readonly IIOHelper _ioHelper;

#if NET472
        public RenderMacroDataEditor(IIOHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }
#else
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IJsonSerializer _jsonSerializer;

        public RenderMacroDataEditor(
            IIOHelper ioHelper,
            ILocalizedTextService localizedTextService,
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer)
        {
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

        public string Group => Constants.Conventions.PropertyGroups.Display;

        public bool IsDeprecated => false;

        public IDictionary<string, object> DefaultConfiguration => default;

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor() => new RenderMacroConfigurationEditor(_ioHelper);

        public IDataValueEditor GetValueEditor()
        {
#if NET472
            return new ReadOnlyDataValueEditor
#else
            return new ReadOnlyDataValueEditor(
                _localizedTextService,
                _shortStringHelper,
                _jsonSerializer)
#endif
            {
                ValueType = ValueTypes.Integer,
                View = _ioHelper.ResolveRelativeOrVirtualUrl(DataEditorViewPath)
            };
        }

        public IDataValueEditor GetValueEditor(object configuration)
        {
            var hideLabel = false;

            if (configuration is Dictionary<string, object> config && config.TryGetValue(HideLabelConfigurationField.HideLabelAlias, out var obj) == true)
            {
                hideLabel = obj.TryConvertTo<bool>().Result;
            }

#if NET472
            return new ReadOnlyDataValueEditor
#else
            return new ReadOnlyDataValueEditor(
                _localizedTextService,
                _shortStringHelper,
                _jsonSerializer)
#endif
            {
                Configuration = configuration,
                HideLabel = hideLabel,
                ValueType = ValueTypes.Integer,
                View = _ioHelper.ResolveRelativeOrVirtualUrl(DataEditorViewPath)
            };
        }
    }
}
