/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.DynamicRoot;
using Umbraco.Cms.Core.DynamicRoot.QuerySteps;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.DeliveryApi;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Community.Contentment.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoContentDataListSource
        : IDataListSource, IDataPickerSource, IDataSourceValueConverter, IDataSourceDeliveryApiValueConverter
    {
        private readonly IApiContentBuilder _apiContentBuilder;
        private readonly IContentmentContentContext _contentmentContentContext;
        private readonly IContentTypeService _contentTypeService;
        private readonly IDynamicRootService _dynamicRootService;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IIOHelper _ioHelper;

        private const string DefaultImageAlias = "image";

        public UmbracoContentDataListSource(
            IApiContentBuilder apiContentBuilder,
            IContentmentContentContext contentmentContentContext,
            IContentTypeService contentTypeService,
            IDynamicRootService dynamicRootService,
            IJsonSerializer jsonSerializer,
            IUmbracoContextAccessor umbracoContextAccessor,
            IIOHelper ioHelper)
        {
            _apiContentBuilder = apiContentBuilder;
            _contentmentContentContext = contentmentContentContext;
            _contentTypeService = contentTypeService;
            _dynamicRootService = dynamicRootService;
            _jsonSerializer = jsonSerializer;
            _umbracoContextAccessor = umbracoContextAccessor;
            _ioHelper = ioHelper;
        }

        public string Name => "Umbraco Content";

        public string Description => "Select a start node to use its children as the data source.";

        public string Icon => "icon-umbraco";

        public Dictionary<string, object>? DefaultValues => default;

        public IEnumerable<ContentmentConfigurationField> Fields => new ContentmentConfigurationField[]
        {
            new ContentmentConfigurationField
            {
                Key = "parentNode",
                Name = "Parent node",
                Description = "Set a parent node to use its child nodes as the data source items.",
                View =  _ioHelper.ResolveRelativeOrVirtualUrl(ContentPickerDataEditor.DataEditorViewPath),
                PropertyEditorUiAlias = ContentPickerDataEditor.DataEditorUiAlias,
            },
            new ContentmentConfigurationField
            {
                Key = "imageAlias",
                Name = "Image alias",
                Description = $"When using the Cards display mode, you can set a thumbnail image by enter the property alias of the media picker. The default alias is '{DefaultImageAlias}'.",
                View =  "textstring",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "sortAlphabetically",
                Name = "Sort alphabetically?",
                Description = "Select to sort the content items in alphabetical order.<br>By default, the order is defined by the Umbraco content sort order.",
                View = "boolean",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
            },
        };

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var start = GetStartContent(config);
            if (start is not null)
            {
                var imageAlias = config.GetValueAs("imageAlias", DefaultImageAlias) ?? DefaultImageAlias;
                var items = start.Children?.Select(x => ToDataListItem(x, imageAlias)) ?? Enumerable.Empty<DataListItem>();

                if (config.TryGetValueAs("sortAlphabetically", out bool sortAlphabetically) == true && sortAlphabetically == true)
                {
                    return items.OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
                }

                return items;
            }

            return Enumerable.Empty<DataListItem>();
        }

        public Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
        {
            if (values?.Any() == true &&
                _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true &&
                umbracoContext.Content != null)
            {
                var preview = true;
                var imageAlias = config.GetValueAs("imageAlias", DefaultImageAlias) ?? DefaultImageAlias;

                return Task.FromResult(values
                    .Select(x => UdiParser.TryParse(x, out GuidUdi? udi) == true ? udi : null)
                    .WhereNotNull()
                    .Select(x => umbracoContext.Content.GetById(preview, x.Guid))
                    .WhereNotNull()
                    .Select(x => ToDataListItem(x, imageAlias)));
            }

            return Task.FromResult(Enumerable.Empty<DataListItem>());
        }

        public Task<PagedViewModel<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "")
        {
            var start = GetStartContent(config);
            if (start != null)
            {
                var items = string.IsNullOrWhiteSpace(query) == false
                    ? start.SearchChildren(query).Select(x => x.Content)
                    : start.Children;

                if (items?.Any() == true)
                {
                    var imageAlias = config.GetValueAs("imageAlias", DefaultImageAlias) ?? DefaultImageAlias;
                    var offset = (pageNumber - 1) * pageSize;

                    if (config.TryGetValueAs("sortAlphabetically", out bool sortAlphabetically) == true && sortAlphabetically == true)
                    {
                        items = items.OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
                    }

                    var results = new PagedViewModel<DataListItem>
                    {
                        Items = items.Skip(offset).Take(pageSize).Select(x => ToDataListItem(x, imageAlias)),
                        Total = pageSize > 0 ? (long)Math.Ceiling(items.Count() / (decimal)pageSize) : 1,
                    };

                    return Task.FromResult(results);
                }
            }

            return Task.FromResult(PagedViewModel<DataListItem>.Empty());
        }

        public Type? GetValueType(Dictionary<string, object>? config) => typeof(IPublishedContent);

        public object? ConvertValue(Type type, string value)
        {
            return UdiParser.TryParse(value, out GuidUdi? udi) == true && udi is not null && _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true
                ? umbracoContext.Content?.GetById(udi.Guid)
                : default;
        }

        public Type? GetDeliveryApiValueType(Dictionary<string, object>? config) => typeof(IApiContent);

        public object? ConvertToDeliveryApiValue(Type type, string value, bool expanding = false)
            => ConvertValue(type, value) is IPublishedContent content ? _apiContentBuilder.Build(content) : default;

        private IPublishedContent? GetStartContent(Dictionary<string, object> config)
        {
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true &&
                umbracoContext.Content is IPublishedContentCache contentCache)
            {
                var preview = true;
                var parentNode = config.GetValueAs("parentNode", string.Empty);

                // Content Picker
                if (UdiParser.TryParse(parentNode, out GuidUdi? udi) == true &&
                    udi is not null &&
                    udi.EntityType == UmbConstants.UdiEntityType.Document &&
                    udi.Guid.Equals(Guid.Empty) == false)
                {
                    return contentCache.GetById(preview, udi.Guid);
                }
                // Dynamic Root
                else if (parentNode?.DetectIsJson() == true)
                {
                    var current = _contentmentContentContext.GetCurrentContent(out var isParent);

                    var model = _jsonSerializer.Deserialize<DynamicRoot>(parentNode);
                    if (model is not null)
                    {
                        var query = new DynamicRootNodeQuery
                        {
                            Context = new DynamicRootContext
                            {
                                CurrentKey = current?.Key,
                                ParentKey = (isParent == true ? current?.Key : current?.Parent?.Key) ?? Guid.Empty
                            },
                            OriginAlias = model.OriginAlias,
                            OriginKey = model.OriginKey,
                            QuerySteps = model.QuerySteps.Select(x => new DynamicRootQueryStep
                            {
                                Alias = x.Alias,
                                AnyOfDocTypeKeys = x.AnyOfDocTypeKeys
                            }),
                        };

                        var startNodes = _dynamicRootService.GetDynamicRootsAsync(query).GetAwaiter().GetResult();
                        if (startNodes?.Any() == true)
                        {
                            return contentCache.GetById(preview, startNodes.First());
                        }
                    }
                }
                // XPath
                else if (string.IsNullOrWhiteSpace(parentNode) == false)
                {
                    IEnumerable<string> getPath(int id) => contentCache.GetById(preview, id)?.Path.ToDelimitedList().Reverse() ?? UmbConstants.System.RootString.AsEnumerableOfOne();
                    bool publishedContentExists(int id) => contentCache.GetById(preview, id) != null;

                    var parsed = _contentmentContentContext.ParseXPathQuery(parentNode, getPath, publishedContentExists);

                    if (string.IsNullOrWhiteSpace(parsed) == false && parsed.StartsWith('$') == false)
                    {
                        //#pragma warning disable CS0618 // Type or member is obsolete
                        //                        return contentCache.GetSingleByXPath(preview, parsed);
                        return default;
                        //#pragma warning restore CS0618 // Type or member is obsolete
                    }
                }
            }

            return default;
        }

        private DataListItem ToDataListItem(IPublishedContent content, string imageAlias = DefaultImageAlias)
        {
            return new DataListItem
            {
                Name = content.Name,
                Description = content.TemplateId > 0 ? content.Url() : string.Empty,
                Disabled = content.IsPublished() == false,
                Icon = content.ContentType.GetIcon(_contentTypeService),
                Properties = new Dictionary<string, object>
                {
                    { DefaultImageAlias, content.Value<IPublishedContent>(imageAlias)?.Url() ?? string.Empty },
                },
                Value = content.GetUdi().ToString(),
            };
        }
    }
}
