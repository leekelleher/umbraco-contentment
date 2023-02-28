/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
#if NET472
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Serialization;
using Umbraco.Core.Services;
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
        internal const string DataEditorDataSourcePreviewViewPath = Constants.Internals.EditorsPathRoot + "data-source.preview.html";
        internal const string DataEditorIcon = "icon-fa fa-list-ul";

        private readonly ConfigurationEditorUtility _utility;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IIOHelper _ioHelper;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IJsonSerializer _jsonSerializer;

        public DataListDataEditor(
            IIOHelper ioHelper,
#if NET472 == false
            IJsonSerializer jsonSerializer,
#endif
            ILocalizedTextService localizedTextService,
            IShortStringHelper shortStringHelper,
            ConfigurationEditorUtility utility)
        {
            _ioHelper = ioHelper;
            _localizedTextService = localizedTextService;
            _shortStringHelper = shortStringHelper;
#if NET472
            _jsonSerializer = DependencyResolver.Current.GetService<IJsonSerializer>();
#else
            _jsonSerializer = jsonSerializer;
#endif
            _utility = utility;
        }

        public string Alias => DataEditorAlias;

        public EditorType Type => EditorType.PropertyValue;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => UmbConstants.PropertyEditors.Groups.Lists;

        public bool IsDeprecated => false;

        public IDictionary<string, object> DefaultConfiguration => new Dictionary<string, object>();

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor() => new DataListConfigurationEditor(_ioHelper, _localizedTextService, _shortStringHelper, _utility);

        public IDataValueEditor GetValueEditor()
        {
            return new DataListDataValueEditor(_localizedTextService, _shortStringHelper, _jsonSerializer)
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

            return new DataListDataValueEditor(_localizedTextService, _shortStringHelper, _jsonSerializer)
            {
                Configuration = configuration,
                ValueType = ValueTypes.Json,
                View = _ioHelper.ResolveRelativeOrVirtualUrl(view ?? DataEditorViewPath),
            };
        }
    }
}
