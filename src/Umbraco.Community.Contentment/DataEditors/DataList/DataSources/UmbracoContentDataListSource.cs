/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private const string _defaultImageAlias = "image";

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

        public Dictionary<string, object> DefaultValues => default;

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
                Description = $"When using the Cards display mode, you can set a thumbnail image by enter the property alias of the media picker. The default alias is '{_defaultImageAlias}'.",
                View =  "textstring",
            }
        };

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var start = GetStartContent(config);
            if (start != null)
            {
                var imageAlias = config.GetValueAs("imageAlias", _defaultImageAlias);
                return start.Children.Select(x => ToDataListItem(x, imageAlias));
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
                var imageAlias = config.GetValueAs("imageAlias", _defaultImageAlias);

                return Task.FromResult(values
                    .Select(x => UdiParser.TryParse(x, out GuidUdi udi) == true ? udi : null)
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
                    var imageAlias = config.GetValueAs("imageAlias", _defaultImageAlias);
                    var offset = (pageNumber - 1) * pageSize;
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

        public object ConvertValue(Type type, string value)
        {
            return UdiParser.TryParse(value, out GuidUdi udi) == true && _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true
                ? umbracoContext.Content.GetById(udi)
                : default;
        }

        private IPublishedContent GetStartContent(Dictionary<string, object> config)
        {
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true &&
                umbracoContext.Content is IPublishedContentCache contentCache)
            {
                var preview = true;
                var parentNode = config.GetValueAs("parentNode", string.Empty);
                if (parentNode.InvariantStartsWith("umb://document/") == false)
                {
                    IEnumerable<string> getPath(int id) => contentCache.GetById(preview, id)?.Path.ToDelimitedList().Reverse();
                    bool publishedContentExists(int id) => contentCache.GetById(preview, id) != null;

                    var parsed = _contentmentContentContext.ParseXPathQuery(parentNode, getPath, publishedContentExists);

                    if (string.IsNullOrWhiteSpace(parsed) == false && parsed.StartsWith("$") == false)
                    {
                        return contentCache.GetSingleByXPath(preview, parsed);
                    }
                }
                else if (UdiParser.TryParse(parentNode, out GuidUdi udi) == true && udi.Guid != Guid.Empty)
                {
                    return contentCache.GetById(preview, udi.Guid);
                }
            }

            return default;
        }

        private DataListItem ToDataListItem(IPublishedContent content, string imageAlias = _defaultImageAlias)
        {
            return new DataListItem
            {
                Name = content.Name,
                Description = content.TemplateId > 0 ? content.Url() : string.Empty,
                Disabled = content.IsPublished() == false,
                Icon = content.ContentType.GetIcon(_contentTypeService),
                Properties = new Dictionary<string, object>
                {
                    { _defaultImageAlias, content.Value<IPublishedContent>(imageAlias)?.Url() },
                },
                Value = content.GetUdi().ToString(),
            };
        }
    }
}
