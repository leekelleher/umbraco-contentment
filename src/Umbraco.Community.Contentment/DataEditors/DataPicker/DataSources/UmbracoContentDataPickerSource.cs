using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Community.Contentment.Services;
using System.Threading.Tasks;
#if NET472
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Core.Xml;
using Umbraco.Web;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Xml;
using Umbraco.Extensions;
using Umbraco.Web;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class UmbracoContentDataPickerSource : IDataPickerSource, IDataPickerSourceValueConverter
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IContentmentContentContext _contentmentContentContext;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IIOHelper _ioHelper;

        public UmbracoContentDataPickerSource(
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

        public string Name => Constants.Internals.DataEditorNamePrefix + "Umbraco Content";

        public string Description => "Select a start node to use its children as the data source.";

        public string Icon => "icon-umbraco";

        public Dictionary<string, object> DefaultValues => default;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "parentNode",
                Name = "Parent node",
                Description = "Set a parent node to use its child nodes as the data source items.",
                View =  _ioHelper.ResolveRelativeOrVirtualUrl("~/App_Plugins/Contentment/editors/content-source.html"),
            },
            new ConfigurationField
            {
                Key = "imageAlias",
                Name = "Image alias",
                Description = "When using the Cards view, you can set a thumbnail image by enter the property alias of the media picker. The default alias is 'image'.",
                View =  "textstring",
            }
        };

        public string Group => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public Task<IEnumerable<DataPickerItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
        {
            if (values?.Any() == true &&
                _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true &&
                umbracoContext.Content != null)
            {
                var preview = true;
                var imageAlias = config.GetValueAs("imageAlias", "image");

                return Task.FromResult(values
                    .Select(x => UdiParser.TryParse(x, out GuidUdi udi) == true ? udi : null)
                    .WhereNotNull()
                    .Select(x => umbracoContext.Content.GetById(preview, x))
                    .WhereNotNull()
                    .Select(x => new DataPickerItem
                    {
                        Name = x.Name,
                        Value = x.GetUdi().ToString(),
                        Icon = x.ContentType.GetIcon(_contentTypeService),
                        Image = x.Value<IPublishedContent>(imageAlias)?.Url(),
                        Description = x.TemplateId > 0 ? x.Url() : string.Empty,
                    }));
            }

            return Task.FromResult(Enumerable.Empty<DataPickerItem>());
        }

        public Task<IEnumerable<DataPickerItem>> SearchAsync(Dictionary<string, object> config, out int totalPages, int pageNumber = 1, int pageSize = 12, string query = "")
        {
            totalPages = -1;

            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true &&
                umbracoContext.Content != null)
            {
                var preview = true;
                var parentNode = config.GetValueAs("parentNode", string.Empty);
                var imageAlias = config.GetValueAs("imageAlias", "image");
                var startNode = default(IPublishedContent);

                if (parentNode.InvariantStartsWith("umb://document/") == false)
                {
                    var nodeContextId = _contentmentContentContext.GetCurrentContentId();

                    IEnumerable<string> getPath(int id) => umbracoContext.Content.GetById(preview, id)?.Path.ToDelimitedList().Reverse();
                    bool publishedContentExists(int id) => umbracoContext.Content.GetById(preview, id) != null;

                    var parsed = UmbracoXPathPathSyntaxParser.ParseXPathQuery(parentNode, nodeContextId, getPath, publishedContentExists);

                    if (string.IsNullOrWhiteSpace(parsed) == false && parsed.StartsWith("$") == false)
                    {
                        startNode = umbracoContext.Content.GetSingleByXPath(preview, parsed);
                    }

                }
                else if (UdiParser.TryParse(parentNode, out GuidUdi udi) == true && udi.Guid != Guid.Empty)
                {
                    startNode = umbracoContext.Content.GetById(preview, udi.Guid);
                }

                if (startNode != null)
                {
                    var items = string.IsNullOrWhiteSpace(query) == false
                        ? startNode.SearchChildren(query).Select(x => x.Content)
                        : startNode.Children;

                    if (items?.Any() == true)
                    {
                        totalPages = (int)Math.Ceiling((double)items.Count() / pageSize);

                        var offset = (pageNumber - 1) * pageSize;

                        return Task.FromResult(items.Skip(offset).Take(pageSize).Select(x => new DataPickerItem
                        {
                            Name = x.Name,
                            Value = x.GetUdi().ToString(),
                            Icon = x.ContentType.GetIcon(_contentTypeService),
                            Image = x.Value<IPublishedContent>(imageAlias)?.Url(),
                            Description = x.TemplateId > 0 ? x.Url() : string.Empty,
                        }));
                    }
                }
            }

            return Task.FromResult(Enumerable.Empty<DataPickerItem>());
        }

        public Type GetValueType(Dictionary<string, object> config) => typeof(IPublishedContent);

        public object ConvertValue(Type type, string value)
        {
            return UdiParser.TryParse(value, out GuidUdi udi) == true && _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true
                ? umbracoContext.Content.GetById(udi)
                : default;
        }
    }
}
