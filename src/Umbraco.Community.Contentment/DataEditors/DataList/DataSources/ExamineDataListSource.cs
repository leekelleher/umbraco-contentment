/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Examine;
using Examine.Search;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Community.Contentment.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class ExamineDataListSource : IContentmentDataSource, IDataPickerSource
    {
        private readonly IContentmentContentContext _contentmentContentContext;
        private readonly IContentTypeService _contentTypeService;
        private readonly ExamineSearcher? _examineSearcher;
        private readonly IIdKeyMap _idKeyMap;
        private readonly ILanguageService _languageService;
        private readonly IMediaTypeService _mediaTypeService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IShortStringHelper _shortStringHelper;

        // NOTE: Umbraco 19 moved Examine into the (optional) Umbraco.Cms.Search.Provider.Examine package.
        // Referencing it directly would enable Examine on every site, so its types are resolved at runtime.
        private static readonly Type? _activeIndexManagerType = Type.GetType("Umbraco.Cms.Search.Provider.Examine.Services.IActiveIndexManager, Umbraco.Cms.Search.Provider.Examine");
        private static readonly MethodInfo? _resolveActiveIndexName = _activeIndexManagerType?.GetMethod("ResolveActiveIndexName", [typeof(string)]);

        private const string _defaultNameField = "Field_" + UmbConstants.Search.FieldNames.Name + "_keywords";
        private const string _defaultNameSearchField = "Field_" + UmbConstants.Search.FieldNames.Name + "_textsr1";
        private const string _defaultValueField = "Field_" + UmbConstants.Search.FieldNames.Id + "_keywords";
        private const string _contentTypeIdField = "Field_" + UmbConstants.Search.FieldNames.ContentTypeId + "_keywords";
        private const string _objectTypeField = "Field_" + UmbConstants.Search.FieldNames.ObjectType + "_keywords";
        private const string _cultureField = "Sys_Culture";

        // Index aliases and field names, from before Umbraco 19, that are still found in stored configuration.
        private static readonly Dictionary<string, string> _legacyIndexAliases = new(StringComparer.OrdinalIgnoreCase)
        {
            { "ExternalIndex", UmbConstants.Search.IndexAliases.PublishedContent },
            { "InternalIndex", UmbConstants.Search.IndexAliases.DraftContent },
            { "MembersIndex", UmbConstants.Search.IndexAliases.DraftMembers },
        };

        private static readonly Dictionary<string, string> _legacyFieldNames = new(StringComparer.OrdinalIgnoreCase)
        {
            { "nodeName", _defaultNameField },
            { "__Key", _defaultValueField },
            // The icon is no longer indexed, (an empty icon field uses the content type's icon).
            { "__Icon", string.Empty },
        };

        private static readonly Dictionary<string, string> _legacyIndexTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            { "content", nameof(UmbracoObjectTypes.Document) },
            { "media", nameof(UmbracoObjectTypes.Media) },
            { "member", nameof(UmbracoObjectTypes.Member) },
        };

        private static readonly Regex _legacyIndexTypeQuery = new($@"__IndexType:""?({string.Join('|', _legacyIndexTypes.Keys)})""?", RegexOptions.IgnoreCase);

        // With zero-downtime indexing, each Umbraco index has two physical indexes (`_a` and `_b`).
        private static readonly Regex _shadowIndexSuffix = new("^(Umb_.+)_[ab]$", RegexOptions.Compiled);

        private readonly Dictionary<string, object> _examineFieldConfig = new()
        {
            {
                Constants.Conventions.ConfigurationFieldAliases.Items,
                new[]
                {
                    _contentTypeIdField,
                    "Field_" + UmbConstants.Search.FieldNames.CreateDate + "_datetimeoffsets",
                    _defaultValueField,
                    "Field_" + UmbConstants.Search.FieldNames.Level + "_integers",
                    _defaultNameField,
                    _defaultNameSearchField,
                    _objectTypeField,
                    "Field_" + UmbConstants.Search.FieldNames.ParentId + "_keywords",
                    "Field_" + UmbConstants.Search.FieldNames.PathIds + "_keywords",
                    "Field_" + UmbConstants.Search.FieldNames.SortOrder + "_integers",
                    "Field_" + UmbConstants.Search.FieldNames.UpdateDate + "_datetimeoffsets",
                    _cultureField,
                }.Select(x => new DataListItem { Name = x, Value = x })
            },
        };

        public ExamineDataListSource(
            IContentmentContentContext contentmentContentContext,
            IContentTypeService contentTypeService,
            IIdKeyMap idKeyMap,
            ILanguageService languageService,
            IMediaTypeService mediaTypeService,
            IServiceProvider serviceProvider,
            IShortStringHelper shortStringHelper)
        {
            _contentmentContentContext = contentmentContentContext;
            _contentTypeService = contentTypeService;
            _examineSearcher = ExamineSearcher.IsAvailable == true ? ExamineSearcher.Create(serviceProvider) : null;
            _idKeyMap = idKeyMap;
            _languageService = languageService;
            _mediaTypeService = mediaTypeService;
            _serviceProvider = serviceProvider;
            _shortStringHelper = shortStringHelper;
        }

        public string Name => "Examine Query";

        public string Description => "Populate the data source from an Examine query.";

        public string Icon => "icon-search";

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public OverlaySize OverlaySize => OverlaySize.Medium;

        public IEnumerable<ContentmentConfigurationField> Fields => new[]
        {
            new ContentmentConfigurationField
            {
                Key = "examineIndex",
                Name = "Examine Index",
                Description = _examineSearcher is null
                    ? "<strong>Examine is not installed.</strong> Install the Examine search provider (<code>Umbraco.Cms.Search.Provider.Examine</code>) to use this data source."
                    : "Select the Examine index.",
                PropertyEditorUiAlias = RadioButtonListDataListEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { DropdownListDataListEditor.AllowEmpty, false },
                    // NOTE: in Contentment v6, the list of Examine Indexes is static at browser load-time. [LK]
                    // Currently unsure whether this needs to be moved to its own data-source; feels overkill.
                    { Constants.Conventions.ConfigurationFieldAliases.Items, GetIndexAliases().Select(x => new DataListItem
                        {
                            Name = x.Replace("Umb_", string.Empty).SplitPascalCasing(_shortStringHelper),
                            Value = x
                        }) },
                }
            },
            new NotesConfigurationField(@"<details class=""well"">
<summary><strong>Do you need help with Lucene query?</strong></summary>
<p>If you need assistance with Lucene query syntax, please refer to this resource on <a href=""https://github.com/mcandre/cheatsheets/blob/main/lucene.md"" target=""_blank""><strong>github.com/mcandre/cheatsheets</strong></a>.</p>
</details>", true),
            new ContentmentConfigurationField
            {
                Key = "luceneQuery",
                Name = "Lucene query",
                Description = "Enter your raw Lucene expression to query Examine with.<br>To make the query contextual using the content's page UDI, you can use C# standard <code>string.Format</code> syntax, e.g. <code>+propertyAlias:\"{0}\"</code>",
                PropertyEditorUiAlias = CodeEditorDataEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { "mode", "text" },
                    { "minLines", 1 },
                    { "maxLines", 5 },
                }
            },
            new ContentmentConfigurationField
            {
                Key = "nameField",
                Name = "Name Field",
                Description = "Enter the field name to select the name from the Examine record.",
                PropertyEditorUiAlias = TextInputDataEditor.DataEditorUiAlias,
                Config = _examineFieldConfig
            },
            new ContentmentConfigurationField
            {
                Key = "valueField",
                Name = "Value Field",
                Description = "Enter the field name to select the value (key) from the Examine record.",
                PropertyEditorUiAlias = TextInputDataEditor.DataEditorUiAlias,
                Config = _examineFieldConfig
            },
            new ContentmentConfigurationField
            {
                Key = "iconField",
                Name = "Icon Field",
                Description = "<em>(optional)</em> Enter the field name to select the icon from the Examine record. When empty, the icon of the content type is used.",
                PropertyEditorUiAlias = TextInputDataEditor.DataEditorUiAlias,
                Config = _examineFieldConfig
            },
            new ContentmentConfigurationField
            {
                Key = "descriptionField",
                Name = "Description Field",
                Description = "<em>(optional)</em> Enter the field name to select the description from the Examine record.",
                PropertyEditorUiAlias = TextInputDataEditor.DataEditorUiAlias,
                Config = _examineFieldConfig
            },
        };

        public Dictionary<string, object> DefaultValues => new()
        {
            { "examineIndex", UmbConstants.Search.IndexAliases.PublishedContent },
            { "luceneQuery", $"+{_objectTypeField}:{nameof(UmbracoObjectTypes.Document)}" },
            { "nameField", _defaultNameField },
            { "valueField", _defaultValueField },
            { "iconField", string.Empty },
            { "descriptionField", string.Empty },
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
            => GetExamineResults(config, pageSize: ExamineSearcher.DefaultMaxResults)?.Items ?? [];

        public Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
            => Task.FromResult(GetExamineResults(config, pageSize: values.Count(), values: values)?.Items ?? []);

        public Task<PagedViewModel<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "", IEnumerable<string>? values = null)
            => Task.FromResult(GetExamineResults(config, pageNumber, pageSize, query));

        private static string MapLegacyValue(Dictionary<string, string> map, string? value)
            => value is not null && map.TryGetValue(value, out var mapped) == true ? mapped : value ?? string.Empty;

        private IEnumerable<string> GetIndexAliases()
            => _examineSearcher is null
                ? []
                : _examineSearcher.GetIndexNames()
                    .Select(x => _shadowIndexSuffix.Replace(x, "$1"))
                    .Distinct()
                    .OrderBy(x => x);

        private string ResolveActiveIndexName(string indexAlias)
            => _resolveActiveIndexName is not null &&
                _activeIndexManagerType is not null &&
                _serviceProvider.GetService(_activeIndexManagerType) is { } activeIndexManager
                    ? _resolveActiveIndexName.Invoke(activeIndexManager, [indexAlias]) as string ?? indexAlias
                    : indexAlias;

        private string GetCultureOrDefault()
            => _contentmentContentContext.GetCurrentCulture() ?? _languageService.GetDefaultIsoCodeAsync().GetAwaiter().GetResult();

        private static string? GetFieldValue(IReadOnlyDictionary<string, IReadOnlyList<string>> result, string field)
            => string.IsNullOrWhiteSpace(field) == false && result.TryGetValue(field, out var values) == true
                ? values.FirstOrDefault()
                : null;

        private string GetContentTypeIcon(IReadOnlyDictionary<string, IReadOnlyList<string>> result, Dictionary<(bool, Guid), string> icons)
        {
            if (Guid.TryParse(GetFieldValue(result, _contentTypeIdField), out var contentTypeKey) == false)
            {
                return UmbConstants.Icons.DefaultIcon;
            }

            var isMedia = GetFieldValue(result, _objectTypeField) == nameof(UmbracoObjectTypes.Media);

            if (icons.TryGetValue((isMedia, contentTypeKey), out var icon) == false)
            {
                icon = (isMedia == true ? _mediaTypeService.Get(contentTypeKey)?.Icon : _contentTypeService.Get(contentTypeKey)?.Icon) ?? UmbConstants.Icons.DefaultIcon;
                icons[(isMedia, contentTypeKey)] = icon;
            }

            return icon;
        }

        private PagedViewModel<DataListItem> GetExamineResults(
            Dictionary<string, object> config,
            int pageNumber = 1,
            int pageSize = 12,
            string? query = default,
            IEnumerable<string>? values = default)
        {
            var examineIndex = config.GetValueAs("examineIndex", UmbConstants.Search.IndexAliases.PublishedContent) ?? UmbConstants.Search.IndexAliases.PublishedContent;
            if (_examineSearcher is not null)
            {
                var luceneQuery = config.GetValueAs("luceneQuery", string.Empty);
                if (string.IsNullOrWhiteSpace(luceneQuery) == false)
                {
                    luceneQuery = _legacyIndexTypeQuery.Replace(luceneQuery, x => $"{_objectTypeField}:{_legacyIndexTypes[x.Groups[1].Value]}");

                    if (luceneQuery.Contains("{0}") == true)
                    {
                        var contentId = _contentmentContentContext.GetCurrentContentId();
                        if (contentId > 0)
                        {
                            var udi = _idKeyMap.GetUdiForId(contentId.Value, UmbracoObjectTypes.Document);
                            if (udi.Success == true)
                            {
                                luceneQuery = string.Format(luceneQuery, udi.Result);
                            }
                        }
                        else
                        {
                            var contentKey = _contentmentContentContext.GetCurrentContentId<Guid?>(out _);
                            if (contentKey.HasValue == true && contentKey.Equals(Guid.Empty) == false)
                            {
                                var udi = new GuidUdi(UmbConstants.UdiEntityType.Document, contentKey.Value);
                                luceneQuery = string.Format(luceneQuery, udi);
                            }
                        }
                    }

                    var nameField = MapLegacyValue(_legacyFieldNames, config.GetValueAs("nameField", _defaultNameField));
                    var valueField = MapLegacyValue(_legacyFieldNames, config.GetValueAs("valueField", _defaultValueField));
                    var iconField = MapLegacyValue(_legacyFieldNames, config.GetValueAs("iconField", string.Empty));
                    var descriptionField = MapLegacyValue(_legacyFieldNames, config.GetValueAs("descriptionField", string.Empty));

                    // Without an icon field, the icon comes from the content type.
                    var useContentTypeIcon = string.IsNullOrWhiteSpace(iconField) == true;

                    // Each culture variant is indexed as a separate document, so limit the results to the current culture.
                    if (luceneQuery.Contains(_cultureField) == false)
                    {
                        luceneQuery += $" +({_cultureField}:\"{GetCultureOrDefault()}\" OR {_cultureField}:\"none\")";
                    }

                    if (string.IsNullOrWhiteSpace(query) == false)
                    {
                        // The name keyword field is an exact match, so search the analyzed text field instead.
                        var nameSearchField = nameField == _defaultNameField ? _defaultNameSearchField : nameField;
                        luceneQuery += $" +({nameSearchField}:\"{query}\" OR {valueField}:\"{query}\")";
                    }

                    if (values?.Any() == true)
                    {
                        luceneQuery += string.Concat($" +({valueField}:\"", string.Join($"\" OR {valueField}:\"", values), "\")");
                    }

                    var offset = (pageNumber - 1) * pageSize;
                    var indexName = ResolveActiveIndexName(MapLegacyValue(_legacyIndexAliases, examineIndex));

                    var results = _examineSearcher.Search(indexName, luceneQuery, nameField, offset, pageSize);

                    if (results?.Total > 0)
                    {
                        var icons = new Dictionary<(bool, Guid), string>();

                        return new PagedViewModel<DataListItem>()
                        {
                            Items = results.Hits.Select(x => new DataListItem
                            {
                                Name = GetFieldValue(x, nameField) ?? GetFieldValue(x, _defaultNameField),
                                Value = GetFieldValue(x, valueField) ?? GetFieldValue(x, _defaultValueField),
                                Icon = useContentTypeIcon == true ? GetContentTypeIcon(x, icons) : GetFieldValue(x, iconField),
                                Description = GetFieldValue(x, descriptionField),
                            }).ToList(),
                            Total = results.Total,
                        };
                    }
                }
            }

            return PagedViewModel<DataListItem>.Empty();
        }

        private sealed record ExamineSearchResults(long Total, IReadOnlyList<IReadOnlyDictionary<string, IReadOnlyList<string>>> Hits);

        // NOTE: This is the only code that references Examine types. Examine is an optional dependency of Contentment,
        // (the `Examine.Core` package reference is not passed on to consumers), so this class must only be touched
        // after `IsAvailable` has been checked. Otherwise the CLR throws a `FileNotFoundException` for `Examine.Core`.
        private sealed class ExamineSearcher
        {
            public const int DefaultMaxResults = QueryOptions.DefaultMaxResults;

            private readonly IExamineManager _examineManager;

            private ExamineSearcher(IExamineManager examineManager) => _examineManager = examineManager;

            public static bool IsAvailable => Type.GetType("Examine.IExamineManager, Examine.Core", throwOnError: false) is not null;

            // `NoInlining` stops the JIT from pulling the Examine types into the caller.
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static ExamineSearcher? Create(IServiceProvider serviceProvider)
                => serviceProvider.GetService<IExamineManager>() is { } examineManager ? new ExamineSearcher(examineManager) : null;

            public IEnumerable<string> GetIndexNames() => _examineManager.Indexes.Select(x => x.Name);

            public ExamineSearchResults? Search(string indexName, string luceneQuery, string sortField, int skip, int take)
            {
                if (_examineManager.TryGetIndex(indexName, out var index) == false)
                {
                    return null;
                }

                var results = index
                    .Searcher
                    .CreateQuery()
                    .NativeQuery(luceneQuery)
                    // NOTE: To enable text field sorting, refer to:
                    // https://shazwazza.github.io/Examine/articles/sorting.html
                    // https://github.com/umbraco/Umbraco-CMS/issues/13681#issuecomment-1384637840
                    .OrderBy(new SortableField(sortField, SortType.String))
                    .Execute(QueryOptions.SkipTake(skip, take));

                return results is null
                    ? null
                    : new ExamineSearchResults(results.TotalItemCount, results.Select(x => x.AllValues).ToList());
            }
        }
    }
}
