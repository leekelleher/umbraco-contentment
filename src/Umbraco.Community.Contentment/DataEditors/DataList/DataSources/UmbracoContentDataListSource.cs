/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Core.Xml;
using Umbraco.Web;
using UmbConstants = Umbraco.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoContentDataListSource : IDataListSource, IDataListSourceValueConverter
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public UmbracoContentDataListSource(IContentTypeService contentTypeService, IUmbracoContextAccessor umbracoContextAccessor)
        {
            _contentTypeService = contentTypeService;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public string Name => "Umbraco Content";

        public string Description => "Select Umbraco content to use its children as a data source.";

        public string Icon => "icon-umbraco";

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ConfigurationField
            {
                Key = "parentNode",
                Name = "Parent node",
                Description = "Set a parent node to use its child nodes as the data source items.",
                View = ContentPickerDataEditor.DataEditorSourceViewPath,
            }
        };

        public Dictionary<string, object> DefaultValues => default;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var preview = true;
            var parentNode = config.GetValueAs("parentNode", string.Empty);
            var startNode = default(IPublishedContent);

            if (parentNode.InvariantStartsWith("umb://document/") == false)
            {
                var nodeContextId = default(int?);
                var umbracoContext = _umbracoContextAccessor.UmbracoContext;

                // NOTE: First we check for "id" (if on a content page), then "parentId" (if editing an element).
                if (int.TryParse(umbracoContext.HttpContext.Request.QueryString.Get("id"), out var currentId) == true)
                {
                    nodeContextId = currentId;
                }
                else if (int.TryParse(umbracoContext.HttpContext.Request.QueryString.Get("parentId"), out var parentId) == true)
                {
                    nodeContextId = parentId;
                }

                if (nodeContextId == -20)
                {
                    // TODO: [UP-FOR-GRABS] If the ID = -20, then we can assume that it's come from Nested Content. What to do? ¯\_(ツ)_/¯
                }

                IEnumerable<string> getPath(int id) => umbracoContext.Content.GetById(preview, id)?.Path.ToDelimitedList().Reverse();
                bool publishedContentExists(int id) => umbracoContext.Content.GetById(preview, id) != null;

                var parsed = UmbracoXPathPathSyntaxParser.ParseXPathQuery(parentNode, nodeContextId, getPath, publishedContentExists);

                if (string.IsNullOrWhiteSpace(parsed) == false && parsed.StartsWith("$") == false)
                {
                    startNode = umbracoContext.Content.GetSingleByXPath(preview, parsed);
                }
            }
            else if (GuidUdi.TryParse(parentNode, out var udi) == true && udi.Guid != Guid.Empty)
            {
                startNode = _umbracoContextAccessor.UmbracoContext.Content.GetById(preview, udi.Guid);
            }

            if (startNode != null)
            {
                return startNode.Children.Select(x => new DataListItem
                {
                    // TODO: [LK:2020-12-03] If multi-lingual is enabled, should the `.Name` take the culture into account?
                    Name = x.Name,
                    Value = Udi.Create(UmbConstants.UdiEntityType.Document, x.Key).ToString(),
                    Icon = ContentTypeCacheHelper.TryGetIcon(x.ContentType.Alias, out var icon, _contentTypeService) == true ? icon : UmbConstants.Icons.Content,
                    Description = x.TemplateId > 0 ? x.Url() : string.Empty,
                    Disabled = x.IsPublished() == false,
                });
            }

            return Enumerable.Empty<DataListItem>();
        }

        public Type GetValueType(Dictionary<string, object> config) => typeof(IPublishedContent);

        public object ConvertValue(Type type, string value)
        {
            return Udi.TryParse(value, out var udi) == true
                ? _umbracoContextAccessor.UmbracoContext.Content.GetById(udi)
                : default;
        }
    }
}
