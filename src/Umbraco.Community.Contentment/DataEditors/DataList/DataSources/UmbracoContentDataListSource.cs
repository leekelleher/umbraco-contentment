/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
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
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoContentDataListSource : BaseUmbracoContentDataListSource, IDataListSource, IDataListSourceValueConverter
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IIOHelper _ioHelper;

#if NET472
        public UmbracoContentDataListSource(
            IContentTypeService contentTypeService,
            IUmbracoContextAccessor umbracoContextAccessor,
            IIOHelper ioHelper) : base(umbracoContextAccessor)
        {
            _contentTypeService = contentTypeService;
            _umbracoContextAccessor = umbracoContextAccessor;
            _ioHelper = ioHelper;
        }
#else
        private readonly IRequestAccessor _requestAccessor;

        public UmbracoContentDataListSource(
            IContentTypeService contentTypeService,
            IRequestAccessor requestAccessor,
            IUmbracoContextAccessor umbracoContextAccessor,
            IIOHelper ioHelper) : base(requestAccessor, umbracoContextAccessor)
        {
            _contentTypeService = contentTypeService;
            _requestAccessor = requestAccessor;
            _umbracoContextAccessor = umbracoContextAccessor;
            _ioHelper = ioHelper;
        }
#endif

        public override string Name => "Umbraco Content";

        public override string Description => "Select a start node to use its children as the data source.";

        public override string Icon => "icon-umbraco";

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ConfigurationField
            {
                Key = "parentNode",
                Name = "Parent node",
                Description = "Set a parent node to use its child nodes as the data source items.",
                View =  _ioHelper.ResolveRelativeOrVirtualUrl(ContentPickerDataEditor.DataEditorSourceViewPath),
            }
        };

        public override Dictionary<string, object> DefaultValues => default;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var preview = true;
            var parentNode = config.GetValueAs("parentNode", string.Empty);
            var startNode = default(IPublishedContent);
            var umbracoContext = _umbracoContextAccessor.GetRequiredUmbracoContext();

            if (parentNode.InvariantStartsWith("umb://document/") == false)
            {
                if (ContextContentId == -20)
                {
                    // TODO: [UP-FOR-GRABS] If the ID = -20, then we can assume that it's come from Nested Content. What to do? ¯\_(ツ)_/¯
                }

                IEnumerable<string> getPath(int id) => umbracoContext.Content.GetById(preview, id)?.Path.ToDelimitedList().Reverse();
                bool publishedContentExists(int id) => umbracoContext.Content.GetById(preview, id) != null;

                var parsed = UmbracoXPathPathSyntaxParser.ParseXPathQuery(parentNode, ContextContentId, getPath, publishedContentExists);

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
            return UdiParser.TryParse(value, out GuidUdi udi) == true
                ? _umbracoContextAccessor.GetRequiredUmbracoContext().Content.GetById(udi)
                : default;
        }
    }
}
