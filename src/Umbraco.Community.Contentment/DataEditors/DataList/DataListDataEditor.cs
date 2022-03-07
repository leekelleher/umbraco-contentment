/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
#if NET472
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Strings;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class DataListDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "DataList";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Data List";
        internal const string DataEditorViewPath = Constants.Internals.EmptyEditorViewPath;
        internal const string DataEditorPreviewViewPath = Constants.Internals.EditorsPathRoot + "data-list.preview.html";
        internal const string DataEditorListEditorViewPath = Constants.Internals.EditorsPathRoot + "data-list.editor.html";
        internal const string DataEditorDataSourcePreviewViewPath = Constants.Internals.EditorsPathRoot + "data-source.preview.html";
        internal const string DataEditorIcon = "icon-fa fa-list-ul";

        private readonly ConfigurationEditorUtility _utility;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IIOHelper _ioHelper;

#if NET472
        public DataListDataEditor(
            ConfigurationEditorUtility utility,
            IShortStringHelper shortStringHelper,
            IIOHelper ioHelper)
        {
            _utility = utility;
            _shortStringHelper = shortStringHelper;
            _ioHelper = ioHelper;
        }
#else
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IJsonSerializer _jsonSerializer;

        public DataListDataEditor(
            ILocalizedTextService localizedTextService,
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer,
            ConfigurationEditorUtility utility,
            IIOHelper ioHelper)
        {
            _localizedTextService = localizedTextService;
            _shortStringHelper = shortStringHelper;
            _jsonSerializer = jsonSerializer;
            _utility = utility;
            _ioHelper = ioHelper;
        }
#endif
        public string Alias => DataEditorAlias;

        public EditorType Type => EditorType.PropertyValue;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => UmbConstants.PropertyEditors.Groups.Lists;

        public bool IsDeprecated => false;

        public IDictionary<string, object> DefaultConfiguration => new Dictionary<string, object>();

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor() => new DataListConfigurationEditor(_utility, _shortStringHelper, _ioHelper);

        public IDataValueEditor GetValueEditor()
        {
#if NET472
            return new DataValueEditor
#else
            return new DataValueEditor(_localizedTextService, _shortStringHelper, _jsonSerializer)
#endif
            {
                ValueType = ValueTypes.Json,
                View = _ioHelper.ResolveRelativeOrVirtualUrl(DataEditorViewPath),
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

#if NET472
            return new DataValueEditor
#else
            return new DataValueEditor(_localizedTextService, _shortStringHelper, _jsonSerializer)
#endif
            {
                Configuration = configuration,
                ValueType = ValueTypes.Json,
                View = _ioHelper.ResolveRelativeOrVirtualUrl(view ?? DataEditorViewPath),
            };
        }
    }
}
