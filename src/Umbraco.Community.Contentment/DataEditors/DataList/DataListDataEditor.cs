/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.IO;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class DataListDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "DataList";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Data List";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "_empty.html";
        internal const string DataEditorPreviewViewPath = Constants.Internals.EditorsPathRoot + "data-list.preview.html";
        internal const string DataEditorListEditorViewPath = Constants.Internals.EditorsPathRoot + "data-list.editor.html";
        internal const string DataEditorIcon = "icon-fa fa-list-ul";

        private readonly IDataTypeService _dataTypeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ConfigurationEditorUtility _utility;
        private readonly IIOHelper _ioHelper;

        public DataListDataEditor(
            IDataTypeService dataTypeService,
            ILocalizationService localizationService,
            ILocalizedTextService localizedTextService,
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer,
            ConfigurationEditorUtility utility,
            IIOHelper ioHelper)
        {
            _dataTypeService = dataTypeService;
            _localizationService = localizationService;
            _localizedTextService = localizedTextService;
            _shortStringHelper = shortStringHelper;
            _jsonSerializer = jsonSerializer;
            _utility = utility;
            _ioHelper = ioHelper;
        }

        public string Alias => DataEditorAlias;

        public EditorType Type => EditorType.PropertyValue;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => Cms.Core.Constants.PropertyEditors.Groups.Lists;

        public bool IsDeprecated => false;

        public IDictionary<string, object> DefaultConfiguration => new Dictionary<string, object>();

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor() => new DataListConfigurationEditor(_utility, _ioHelper, _shortStringHelper);

        public IDataValueEditor GetValueEditor()
        {
            return new DataValueEditor(
                _dataTypeService,
                _localizationService,
                _localizedTextService,
                _shortStringHelper,
                _jsonSerializer)
            {
                ValueType = ValueTypes.Json,
                View = DataEditorViewPath,
            };
        }

        public IDataValueEditor GetValueEditor(object configuration)
        {
            var view = default(string);

            if (configuration is Dictionary<string, object> config &&
                config.TryGetValueAs(DataListConfigurationEditor.ListEditor, out JArray array) == true &&
                array.Count > 0 &&
                array[0] is JObject item)
            {
                // NOTE: Patches a breaking-change. I'd renamed `type` to become `key`. [LK:2020-04-03]
                if (item.ContainsKey("key") == false && item.ContainsKey("type") == true)
                {
                    item.Add("key", item["type"]);
                    item.Remove("type");
                }

                var editor = _utility.GetConfigurationEditor<IDataListEditor>(item.Value<string>("key"));
                if (editor != null)
                {
                    view = editor.View;
                }
            }

            return new DataValueEditor(
                _dataTypeService,
                _localizationService,
                _localizedTextService,
                _shortStringHelper,
                _jsonSerializer)
            {
                Configuration = configuration,
                ValueType = ValueTypes.Json,
                View = view ?? DataEditorViewPath,
            };
        }
    }
}
