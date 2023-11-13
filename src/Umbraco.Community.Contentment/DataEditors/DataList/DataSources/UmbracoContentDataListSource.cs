/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Community.Contentment.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoContentDataListSource : IDataListSource, IDataPickerSource, IDataSourceValueConverter
    {
        private readonly IContentmentContentContext _contentmentContentContext;
        private readonly IContentTypeService _contentTypeService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IIOHelper _ioHelper;

        private const string DefaultImageAlias = "image";

        public UmbracoContentDataListSource(
            IContentmentContentContext contentmentContentContext,
            IContentTypeService contentTypeService,
            IUmbracoContextAccessor umbracoContextAccessor,
            IIOHelper ioHelper)
        {
            _contentmentContentContext = contentmentContentContext;
            _contentTypeService = contentTypeService;
            _umbracoContextAccessor = umbracoContextAccessor;
            _ioHelper = ioHelper;
        }

        public string Name => "Umbraco Content";

        public string Description => "Select a start node to use its children as the data source.";

        public string Icon => "icon-umbraco";

        public Dictionary<string, object>? DefaultValues => default;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ConfigurationField
            {
                Key = "parentNode",
                Name = "Parent node",
                Description = "Set a parent node to use its child nodes as the data source items.",
                View =  _ioHelper.ResolveRelativeOrVirtualUrl(ContentPickerDataEditor.DataEditorSourceViewPath),
            },
            new ConfigurationField
            {
                Key = "imageAlias",
                Name = "Image alias",
                Description = $"When using the Cards display mode, you can set a thumbnail image by enter the property alias of the media picker. The default alias is '{DefaultImageAlias}'.",
                View =  "textstring",
            },
            new ConfigurationField
            {
                Key = "sortAlphabetically",
                Name = "Sort alphabetically?",
                Description = "Select to sort the content items in alphabetical order.<br>By default, the order is defined by the Umbraco content sort order.",
                View = "boolean"
            },
        };

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var start = GetStartContent(config);
            if (start != null)
            {
                var imageAlias = config.GetValueAs("imageAlias", DefaultImageAlias) ?? DefaultImageAlias;
                var items = start.Children.Select(x => ToDataListItem(x, imageAlias));

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
                    .Select(x => umbracoContext.Content.GetById(preview, x))
                    .WhereNotNull()
                    .Select(x => ToDataListItem(x, imageAlias)));
            }

            return Task.FromResult(Enumerable.Empty<DataListItem>());
        }

        public Task<PagedResult<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "")
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

                    var results = new PagedResult<DataListItem>(items.Count(), pageNumber, pageSize)
                    {
                        Items = items
                            .Skip(offset)
                            .Take(pageSize)
                            .Select(x => ToDataListItem(x, imageAlias))
                    };

                    return Task.FromResult(results);
                }
            }

            return Task.FromResult(new PagedResult<DataListItem>(-1, pageNumber, pageSize));
        }

        public Type GetValueType(Dictionary<string, object> config) => typeof(IPublishedContent);

        public object? ConvertValue(Type type, string value)
        {
            return UdiParser.TryParse(value, out var udi) == true && udi is not null && _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true
                ? umbracoContext.Content?.GetById(udi)
                : default;
        }

        private IPublishedContent? GetStartContent(Dictionary<string, object> config)
        {
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true &&
                umbracoContext.Content is IPublishedContentCache contentCache)
            {
                var preview = true;
                var parentNode = config.GetValueAs("parentNode", string.Empty);
                if (parentNode?.InvariantStartsWith("umb://document/") == false)
                {
                    IEnumerable<string> getPath(int id) => contentCache.GetById(preview, id)?.Path.ToDelimitedList().Reverse() ?? UmbConstants.System.RootString.AsEnumerableOfOne();
                    bool publishedContentExists(int id) => contentCache.GetById(preview, id) != null;

                    var parsed = _contentmentContentContext.ParseXPathQuery(parentNode, getPath, publishedContentExists);

                    if (string.IsNullOrWhiteSpace(parsed) == false && parsed.StartsWith('$') == false)
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        return contentCache.GetSingleByXPath(preview, parsed);
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                }
                else if (UdiParser.TryParse(parentNode, out GuidUdi? udi) == true && udi.Guid != Guid.Empty)
                {
                    return contentCache.GetById(preview, udi.Guid);
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
