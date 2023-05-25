/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Community.Contentment.Services;
#if NET472
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoContentDataListSource : IDataListSource, IDataSourceValueConverter
    {
        private readonly IContentmentContentContext _contentmentContentContext;
        private readonly IContentTypeService _contentTypeService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IIOHelper _ioHelper;

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
            }
        };

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true)
            {
                var parentNode = config.GetValueAs("parentNode", string.Empty);
                var preview = true;
                var startNode = default(IPublishedContent);

                if (parentNode.InvariantStartsWith("umb://document/") == false)
                {
                    IEnumerable<string> getPath(int id) => umbracoContext.Content.GetById(preview, id)?.Path.ToDelimitedList().Reverse();
                    bool publishedContentExists(int id) => umbracoContext.Content.GetById(preview, id) != null;

                    var parsed = _contentmentContentContext.ParseXPathQuery(parentNode, getPath, publishedContentExists);

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
                    return startNode.Children.Select(x => new DataListItem
                    {
                        // TODO: [UP-FOR-GRABS] If multi-lingual is enabled, should the `.Name` take the culture into account?
                        Name = x.Name,
                        Value = Udi.Create(UmbConstants.UdiEntityType.Document, x.Key).ToString(),
                        Icon = ContentTypeCacheHelper.TryGetIcon(x.ContentType.Alias, out var icon, _contentTypeService) == true ? icon : UmbConstants.Icons.Content,
                        Description = x.TemplateId > 0 ? x.Url() : string.Empty,
                        Disabled = x.IsPublished() == false,
                    });
                }
            }

            return Enumerable.Empty<DataListItem>();
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
