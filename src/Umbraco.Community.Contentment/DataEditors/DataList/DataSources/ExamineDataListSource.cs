﻿/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Examine;
using Examine.Search;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class ExamineDataListSource : IDataListSource
    {
        private readonly IExamineManager _examineManager;
        private readonly IIdKeyMap _idKeyMap;
        private readonly IIOHelper _ioHelper;
        private readonly IRequestAccessor _requestAccessor;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        private const string _defaultNameField = UmbracoExamineFieldNames.NodeNameFieldName;
        private const string _defaultValueField = UmbracoExamineFieldNames.NodeKeyFieldName;
        private const string _defaultIconField = UmbracoExamineFieldNames.IconFieldName;

        private readonly Dictionary<string, object> _examineFieldConfig = new Dictionary<string, object>
        {
            {
                Constants.Conventions.ConfigurationFieldAliases.Items,
                new[]
                {
                    UmbracoExamineFieldNames.CategoryFieldName,
                    UmbracoExamineFieldNames.ItemIdFieldName,
                    UmbracoExamineFieldNames.ItemTypeFieldName,
                    UmbracoExamineFieldNames.IconFieldName,
                    UmbracoExamineFieldNames.IndexPathFieldName,
                    UmbracoExamineFieldNames.NodeKeyFieldName,
                    UmbracoExamineFieldNames.PublishedFieldName,
                    UmbracoExamineFieldNames.UmbracoFileFieldName,
                    "createDate",
                    "creatorID",
                    "creatorName",
                    "icon",
                    "id",
                    "level",
                    _defaultNameField,
                    "nodeType",
                    "parentID",
                    "path",
                    "templateID",
                    "updateDate",
                    "urlName",
                    "writerID",
                    "writerName",
                }.Select(x => new DataListItem { Name = x, Value = x })
            },
        };

        public ExamineDataListSource(
            IExamineManager examineManager,
            IIdKeyMap idKeyMap,
            IIOHelper ioHelper,
            IRequestAccessor requestAccessor,
            IShortStringHelper shortStringHelper,
            IUmbracoContextAccessor umbracoContextAccessor)
        {
            _examineManager = examineManager;
            _idKeyMap = idKeyMap;
            _ioHelper = ioHelper;
            _requestAccessor = requestAccessor;
            _shortStringHelper = shortStringHelper;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public string Name => "Examine Query";

        public string Description => "Populate the data source from an Examine query.";

        public string Icon => "icon-search";

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "examineIndex",
                Name = "Examine Index",
                Description = "Select the Examine index.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(DropdownListDataListEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { DropdownListDataListEditor.AllowEmpty, Constants.Values.False },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, _examineManager.Indexes.OrderBy(x => x.Name).Select(x => new DataListItem
                        {
                            Name = x.Name.SplitPascalCasing(_shortStringHelper),
                            Value = x.Name
                        }) },
                }
            },
            new NotesConfigurationField(_ioHelper, @"<details class=""well well-small"">
<summary><strong>Do you need help with Lucene query?</strong></summary>
<p>If you need assistance with Lucene query syntax, please refer to this resource on <a href=""https://our.umbraco.com/documentation/reference/searching/examine/overview-explanation#power-searching-with-raw-lucene-queries"" target=""_blank""><strong>our.umbraco.com</strong></a>.</p>
</details>", true),
            new ConfigurationField
            {
                Key = "luceneQuery",
                Name = "Lucene query",
                Description = "Enter your raw Lucene expression to query Examine with.<br>To make the query contextual using the content's page UDI, you can use C# standard <code>string.Format</code> syntax, e.g. <code>+propertyAlias:\"{0}\"</code>",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(CodeEditorDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { CodeEditorConfigurationEditor.Mode, "text" },
                    { CodeEditorConfigurationEditor.MinLines, 1 },
                    { CodeEditorConfigurationEditor.MaxLines, 5 },
                }
            },
            new ConfigurationField
            {
                Key = "nameField",
                Name = "Name Field",
                Description = "Enter the field name to select the name from the Examine record.",
                View =  _ioHelper.ResolveRelativeOrVirtualUrl(TextInputDataEditor.DataEditorViewPath),
                Config = _examineFieldConfig
            },
            new ConfigurationField
            {
                Key = "valueField",
                Name = "Value Field",
                Description = "Enter the field name to select the value (key) from the Examine record.",
                View =  _ioHelper.ResolveRelativeOrVirtualUrl(TextInputDataEditor.DataEditorViewPath),
                Config = _examineFieldConfig
            },
            new ConfigurationField
            {
                Key = "iconField",
                Name = "Icon Field",
                Description = "<em>(optional)</em> Enter the field name to select the icon from the Examine record.",
                View =  _ioHelper.ResolveRelativeOrVirtualUrl(TextInputDataEditor.DataEditorViewPath),
                Config = _examineFieldConfig
            },
            new ConfigurationField
            {
                Key = "descriptionField",
                Name = "Description Field",
                Description = "<em>(optional)</em> Enter the field name to select the description from the Examine record.",
                View =  _ioHelper.ResolveRelativeOrVirtualUrl(TextInputDataEditor.DataEditorViewPath),
                Config = _examineFieldConfig
            },
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "examineIndex", UmbConstants.UmbracoIndexes.ExternalIndexName },
            { "luceneQuery", "+__IndexType:content" },
            { "nameField", _defaultNameField },
            { "valueField", _defaultValueField },
            { "iconField", _defaultIconField },
            { "descriptionField", string.Empty },
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var examineIndex = config.GetValueAs("examineIndex", UmbConstants.UmbracoIndexes.ExternalIndexName);
            if (_examineManager.TryGetIndex(examineIndex, out var index) == true)
            {
                var luceneQuery = config.GetValueAs("luceneQuery", string.Empty);
                if (string.IsNullOrWhiteSpace(luceneQuery) == false)
                {
                    if (luceneQuery.Contains("{0}") == true)
                    {
                        if (int.TryParse(_requestAccessor.GetQueryStringValue("id"), out var currentId) == true)
                        {
                            var udi = _idKeyMap.GetUdiForId(currentId, UmbracoObjectTypes.Document);
                            if (udi.Success == true)
                            {
                                luceneQuery = string.Format(luceneQuery, udi.Result);
                            }
                        }
                    }

                    var nameField = config.GetValueAs("nameField", _defaultNameField);
                    var valueField = config.GetValueAs("valueField", _defaultValueField);
                    var iconField = config.GetValueAs("iconField", _defaultIconField);
                    var descriptionField = config.GetValueAs("descriptionField", string.Empty);

                    var results = index
                        .Searcher
                        .CreateQuery()
                        .NativeQuery(luceneQuery)
                        // NOTE: For any `OrderBy` complaints, refer to: https://github.com/Shazwazza/Examine/issues/126
                        .OrderBy(new SortableField(nameField, SortType.String))
                        .Execute();

                    if (results?.TotalItemCount > 0)
                    {
                        return results.Select(x => new DataListItem
                        {
                            Name = x.Values.ContainsKey(nameField) == true ? x.Values[nameField] : x.Values[_defaultNameField],
                            Value = x.Values.ContainsKey(valueField) == true ? x.Values[valueField] : x.Values[_defaultValueField],
                            Icon = x.Values.ContainsKey(iconField) == true ? x.Values[iconField] : x.Values[_defaultIconField],
                            Description = x.Values.ContainsKey(descriptionField) == true ? x.Values[descriptionField] : null,
                        });
                    }
                }
            }

            return Enumerable.Empty<DataListItem>();
        }
    }
}
