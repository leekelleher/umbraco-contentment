/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
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
        internal const string DataEditorViewPath = Constants.Internals.EmptyEditorViewPath;
        internal const string DataEditorOverlayViewPath = Constants.Internals.EditorsPathRoot + "data-picker.overlay.html";
        internal const string DataEditorIcon = "icon-fa-arrow-pointer";
        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "DataPicker";

        private readonly IIOHelper _ioHelper;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly ConfigurationEditorUtility _utility;
        private readonly IJsonSerializer _jsonSerializer;

        public DataPickerDataEditor(
            IIOHelper ioHelper,
            IJsonSerializer jsonSerializer,
            IShortStringHelper shortStringHelper,
            ConfigurationEditorUtility utility)
        {
            _ioHelper = ioHelper;
            _jsonSerializer = jsonSerializer;
            _shortStringHelper = shortStringHelper;
            _utility = utility;
        }

        public string Alias => DataEditorAlias;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => UmbConstants.PropertyEditors.Groups.Lists;

        public bool IsDeprecated => false;

        public IDictionary<string, object>? DefaultConfiguration => default;

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor() => new DataPickerConfigurationEditor(_ioHelper, _utility);

        public IDataValueEditor GetValueEditor()
        {
            return new DataValueEditor(_shortStringHelper, _jsonSerializer)
            {
                ValueType = ValueTypes.Json,
                //View = _ioHelper.ResolveRelativeOrVirtualUrl(DataEditorViewPath),
            };
        }

        public IDataValueEditor GetValueEditor(object? configuration)
        {
            //var view = default(string);

            //if (configuration is Dictionary<string, object> config)
            //{
            //    if (config.TryGetValueAs(DataPickerConfigurationEditor.DisplayMode, out JArray? array1) == true &&
            //        array1?.Count > 0 &&
            //        array1[0] is JObject item1 &&
            //        item1.Value<string>("key") is string key1)
            //    {
            //        var displayMode = _utility.GetConfigurationEditor<IDataPickerDisplayMode>(key1);
            //        if (displayMode != null)
            //        {
            //            view = displayMode.View;
            //        }
            //    }
            //}

            return new DataValueEditor(_shortStringHelper, _jsonSerializer)
            {
                ConfigurationObject = configuration,
                ValueType = ValueTypes.Json,
                //View = _ioHelper.ResolveRelativeOrVirtualUrl(view ?? DataEditorViewPath),
            };
        }
    }
}
