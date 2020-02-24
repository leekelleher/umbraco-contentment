/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Xml;
using Umbraco.Web;
using UmbConstants = Umbraco.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Core.Composing.HideFromTypeFinder]
    public sealed class UmbracoContentXPathDataListSource : IDataListSource
    {
        private readonly IUmbracoContextAccessor _accessor;

        public UmbracoContentXPathDataListSource(IUmbracoContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string Name => "Umbraco Content XPath";

        public string Description => "Select content from an XPath expression as the data source.";

        public string Icon => UmbConstants.Icons.Content;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ConfigurationField
            {
                Key = "xpath",
                Name = "XPath expression",
                Description = "Enter an XPath expression to select the content.",
                View = "textstring"
            }
        };

        public Dictionary<string, object> DefaultValues => default;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var xpath = config.GetValueAs("xpath", string.Empty);
            if (string.IsNullOrWhiteSpace(xpath) == false)
            {
                var umbracoContext = _accessor.UmbracoContext;

                var nodeContextId = int.TryParse(umbracoContext.HttpContext.Request.QueryString.Get("id"), out var currentId)
                    ? currentId
                    : default(int?);

                IEnumerable<string> getPath(int id) => umbracoContext.Content.GetById(id).Path.ToDelimitedList().Reverse();
                bool publishedContentExists(int id) => umbracoContext.Content.GetById(id) != null;

                var parsed = UmbracoXPathPathSyntaxParser.ParseXPathQuery(xpath, nodeContextId, getPath, publishedContentExists);

                if (string.IsNullOrWhiteSpace(parsed) == false && parsed.StartsWith("$") == false)
                {
                    return umbracoContext
                        .Content
                        .GetByXPath(parsed)
                        .Select(x => new DataListItem
                        {
                            Name = x.Name,
                            Value = Udi.Create(UmbConstants.UdiEntityType.Document, x.Key).ToString(),
                            Icon = UmbConstants.Icons.Content,
                            Description = x.Url
                        });
                }
            }

            return Enumerable.Empty<DataListItem>();
        }
    }
}
