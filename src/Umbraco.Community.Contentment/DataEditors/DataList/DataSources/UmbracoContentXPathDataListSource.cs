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
    public sealed class UmbracoContentXPathDataListSource : IDataListSource, IDataListSourceValueConverter
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public UmbracoContentXPathDataListSource(IContentTypeService contentTypeService, IUmbracoContextAccessor umbracoContextAccessor)
        {
            _contentTypeService = contentTypeService;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public string Name => "Umbraco Content by XPath";

        public string Description => "Use an XPath query to select Umbraco content to use as a data source.";

        public string Icon => "icon-fa fa-file-code-o";

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ConfigurationField
            {
                Key = "xpath",
                Name = "XPath",
                Description = "Enter the XPath expression to select the content.",
                View = "textstring",
            },
            new NotesConfigurationField($@"<details class=""well well-small"">
<summary><strong>Do you need help with XPath expressions?</strong></summary>
<p>If you need assistance with XPath syntax in general, please refer to this resource: <a href=""https://www.w3schools.com/xml/xpath_intro.asp"" target=""_blank""><strong>w3schools.com/xml</strong></a>.</p>
<hr>
<p>For querying Umbraco content with XPath, you can make it context-aware queries by using one of the pre-defined placeholders.</p>
<p>Placeholders find the nearest published content ID and run the XPath query from there. For instance:</p>
<pre><code>$site/newsListingPage</code></pre>
<p>This query will try to get the current website page (at level 1), then find the first page of type `newsListingPage`.</p>
<dl>
<dt>Available placeholders:</dt>
<dd><code>$current</code> - current page or closest ancestor.</dd>
<dd><code>$parent</code> - parent page or closest ancestor.</dd>
<dd><code>$root</code> - root page in the content tree.</dd>
<dd><code>$site</code> - ancestor page located at level 1.</dd>
</dl>
<hr />
<p><strong>Please note,</strong> this data source will not work if used within a 'Nested Content' element type. <strong><em>This is a known issue.</em></strong> <a href=""{Constants.Package.RepositoryUrl}/issues/30#issuecomment-668684508"" target=""_blank"" rel=""noopener"">Please see GitHub issue #30 for details.</a></p>
</details>", true),
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "xpath", "/root/*[@level = 1]/*[@isDoc]" },
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var xpath = config.GetValueAs("xpath", string.Empty);

            if (string.IsNullOrWhiteSpace(xpath) == false)
            {
                var nodeContextId = default(int?);
                var preview = true;
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

                IEnumerable<string> getPath(int id) => umbracoContext.Content.GetById(preview, id)?.Path.ToDelimitedList().Reverse();
                bool publishedContentExists(int id) => umbracoContext.Content.GetById(preview, id) != null;

                var parsed = UmbracoXPathPathSyntaxParser.ParseXPathQuery(xpath, nodeContextId, getPath, publishedContentExists);

                if (string.IsNullOrWhiteSpace(parsed) == false && parsed.StartsWith("$") == false)
                {
                    return umbracoContext.Content.GetByXPath(preview, parsed)
                        .Select(x => new DataListItem
                        {
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
            return Udi.TryParse(value, out var udi) == true
                ? _umbracoContextAccessor.UmbracoContext.Content.GetById(udi)
                : default;
        }
    }
}
