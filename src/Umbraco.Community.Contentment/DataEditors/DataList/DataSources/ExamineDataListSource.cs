/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Examine;
using Examine.Search;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Community.Contentment.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class ExamineDataListSource : IDataListSource, IDataPickerSource
    {
        private readonly IContentmentContentContext _contentmentContentContext;
        private readonly IExamineManager _examineManager;
        private readonly IIdKeyMap _idKeyMap;
        private readonly IShortStringHelper _shortStringHelper;

        private const string _defaultNameField = UmbracoExamineFieldNames.NodeNameFieldName;
        private const string _defaultValueField = UmbracoExamineFieldNames.NodeKeyFieldName;
        private const string _defaultIconField = UmbracoExamineFieldNames.IconFieldName;

        private readonly Dictionary<string, object> _examineFieldConfig = new()
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
            IContentmentContentContext contentmentContentContext,
            IExamineManager examineManager,
            IIdKeyMap idKeyMap,
            IShortStringHelper shortStringHelper)
        {
            _contentmentContentContext = contentmentContentContext;
            _examineManager = examineManager;
            _idKeyMap = idKeyMap;
            _shortStringHelper = shortStringHelper;
        }

        public string Name => "Examine Query";

        public string Description => "Populate the data source from an Examine query.";

        public string Icon => "icon-search";

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<ContentmentConfigurationField> Fields => new[]
        {
            new ContentmentConfigurationField
            {
                Key = "examineIndex",
                Name = "Examine Index",
                Description = "Select the Examine index.",
                PropertyEditorUiAlias = "Umb.Contentment.PropertyEditorUi.RadioButtonList",
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
            new NotesConfigurationField(@"<details class=""well well-small"">
<summary><strong>Do you need help with Lucene query?</strong></summary>
<p>If you need assistance with Lucene query syntax, please refer to this resource on <a href=""https://our.umbraco.com/documentation/reference/searching/examine/overview-explanation#power-searching-with-raw-lucene-queries"" target=""_blank""><strong>our.umbraco.com</strong></a>.</p>
</details>", true),
            new ContentmentConfigurationField
            {
                Key = "luceneQuery",
                Name = "Lucene query",
                Description = "Enter your raw Lucene expression to query Examine with.<br>To make the query contextual using the content's page UDI, you can use C# standard <code>string.Format</code> syntax, e.g. <code>+propertyAlias:\"{0}\"</code>",
                PropertyEditorUiAlias ="Umb.Contentment.PropertyEditorUi.CodeEditor",
                Config = new Dictionary<string, object>
                {
                    { CodeEditorConfigurationEditor.Mode, "text" },
                    { CodeEditorConfigurationEditor.MinLines, 1 },
                    { CodeEditorConfigurationEditor.MaxLines, 5 },
                }
            },
            new ContentmentConfigurationField
            {
                Key = "nameField",
                Name = "Name Field",
                Description = "Enter the field name to select the name from the Examine record.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
                Config = _examineFieldConfig
            },
            new ContentmentConfigurationField
            {
                Key = "valueField",
                Name = "Value Field",
                Description = "Enter the field name to select the value (key) from the Examine record.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
                Config = _examineFieldConfig
            },
            new ContentmentConfigurationField
            {
                Key = "iconField",
                Name = "Icon Field",
                Description = "<em>(optional)</em> Enter the field name to select the icon from the Examine record.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
                Config = _examineFieldConfig
            },
            new ContentmentConfigurationField
            {
                Key = "descriptionField",
                Name = "Description Field",
                Description = "<em>(optional)</em> Enter the field name to select the description from the Examine record.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
                Config = _examineFieldConfig
            },
        };

        public Dictionary<string, object> DefaultValues => new()
        {
            { "examineIndex", UmbConstants.UmbracoIndexes.ExternalIndexName },
            { "luceneQuery", "+__IndexType:content" },
            { "nameField", _defaultNameField },
            { "valueField", _defaultValueField },
            { "iconField", _defaultIconField },
            { "descriptionField", string.Empty },
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
            => GetExamineResults(config, pageSize: QueryOptions.DefaultMaxResults)?.Items ?? [];

        public Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
            => Task.FromResult(GetExamineResults(config, pageSize: values.Count(), values: values)?.Items ?? []);

        public Task<PagedViewModel<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "")
            => Task.FromResult(GetExamineResults(config, pageNumber, pageSize, query));

        private PagedViewModel<DataListItem> GetExamineResults(
            Dictionary<string, object> config,
            int pageNumber = 1,
            int pageSize = 12,
            string? query = default,
            IEnumerable<string>? values = default)
        {
            var examineIndex = config.GetValueAs("examineIndex", UmbConstants.UmbracoIndexes.ExternalIndexName);
            if (_examineManager.TryGetIndex(examineIndex, out var index) == true)
            {
                var luceneQuery = config.GetValueAs("luceneQuery", string.Empty);
                if (string.IsNullOrWhiteSpace(luceneQuery) == false)
                {
                    if (luceneQuery.Contains("{0}") == true)
                    {
                        var contentId = _contentmentContentContext.GetCurrentContentId();
                        if (contentId.HasValue == true)
                        {
                            var udi = _idKeyMap.GetUdiForId(contentId.Value, UmbracoObjectTypes.Document);
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

                    if (string.IsNullOrWhiteSpace(query) == false)
                    {
                        luceneQuery += $" +({nameField}:\"{query}\" OR {valueField}:\"{query}\")";
                    }

                    if (values?.Any() == true)
                    {
                        luceneQuery += string.Concat($" +({valueField}:\"", string.Join($"\" OR {valueField}:\"", values), "\")");
                    }

                    var offset = (pageNumber - 1) * pageSize;
                    var queryOptions = QueryOptions.SkipTake(offset, pageSize);

                    var results = index
                        .Searcher
                        .CreateQuery()
                        .NativeQuery(luceneQuery)
                        // NOTE: To enable text field sorting, refer to:
                        // https://shazwazza.github.io/Examine/articles/sorting.html
                        // https://github.com/umbraco/Umbraco-CMS/issues/13681#issuecomment-1384637840
                        .OrderBy(new SortableField(nameField, SortType.String))
                        .Execute(queryOptions);

                    if (results?.TotalItemCount > 0)
                    {
                        return new PagedViewModel<DataListItem>()
                        {
                            Items = results.Select(x => new DataListItem
                            {
                                Name = x.Values.ContainsKey(nameField) == true ? x.Values[nameField] : x.Values[_defaultNameField],
                                Value = x.Values.ContainsKey(valueField) == true ? x.Values[valueField] : x.Values[_defaultValueField],
                                Icon = x.Values.ContainsKey(iconField) == true ? x.Values[iconField] : x.Values[_defaultIconField],
                                Description = x.Values.ContainsKey(descriptionField) == true ? x.Values[descriptionField] : null,
                            }),
                            Total = results.TotalItemCount,
                        };
                    }
                }
            }

            return PagedViewModel<DataListItem>.Empty();
        }
    }
}
