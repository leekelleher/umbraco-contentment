﻿/* Copyright © 2020 Lee Kelleher.
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

namespace Umbraco.Community.Contentment.DataEditors
{
    [Core.Composing.HideFromTypeFinder]
    internal sealed class UmbracoContentXPathDataListSource : IDataListSource, IDataListSourceValueConverter
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public UmbracoContentXPathDataListSource(IContentTypeService contentTypeService, IUmbracoContextAccessor umbracoContextAccessor)
        {
            _contentTypeService = contentTypeService;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public string Name => "Umbraco Content: XPath";

        public string Description => "Use an XPath query to select Umbraco content to use as a data source.";

        public string Icon => "icon-umbraco";

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
            new NotesConfigurationField(@"<details class=""well well-small"">
<summary><strong>Do you need help with XPath expressions?</strong></summary>
<div class=""mt3"">
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
<p><strong>Please note,</strong> when using an XPath query, this data source will not work if used within a Nested Content element type. <a href=""https://github.com/leekelleher/umbraco-contentment/issues/30#issuecomment-668684508"" target=""_blank"" rel=""noopener""><em>This is a known issue.</em></a></p>
</div>
</details>", true),
        };

        public Dictionary<string, object> DefaultValues => default;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var items = new List<IPublishedContent>();

            var xpath = config.GetValueAs("xpath", string.Empty);

            if (string.IsNullOrWhiteSpace(xpath) == false)
            {
                var preview = true;
                var umbracoContext = _umbracoContextAccessor.UmbracoContext;

                // NOTE: First we check for "id" (if on a content page), then "parentId" (if editing an element).
                var nodeContextId = int.TryParse(umbracoContext.HttpContext.Request.QueryString.Get("id"), out var currentId)
                    ? currentId
                    : int.TryParse(umbracoContext.HttpContext.Request.QueryString.Get("parentId"), out var parentId)
                        ? parentId
                        : default(int?);

                IEnumerable<string> getPath(int id) => umbracoContext.Content.GetById(preview, id)?.Path.ToDelimitedList().Reverse();
                bool publishedContentExists(int id) => umbracoContext.Content.GetById(preview, id) != null;

                var parsed = UmbracoXPathPathSyntaxParser.ParseXPathQuery(xpath, nodeContextId, getPath, publishedContentExists);

                if (string.IsNullOrWhiteSpace(parsed) == false && parsed.StartsWith("$") == false)
                {
                    items.AddRange(umbracoContext.Content.GetByXPath(preview, parsed));
                }
            }

            return items.Select(x => new DataListItem
            {
                Name = x.Name,
                Value = Udi.Create(Core.Constants.UdiEntityType.Document, x.Key).ToString(),
                Icon = ContentTypeCacheHelper.TryGetIcon(x.ContentType.Alias, out var icon, _contentTypeService) ? icon : Core.Constants.Icons.Content,
                Description = x.TemplateId > 0 ? x.Url : string.Empty,
                Disabled = x.IsPublished() == false,
            });
        }

        public Type GetValueType(Dictionary<string, object> config)
        {
            return typeof(IPublishedContent);
        }

        public object ConvertValue(Type type, string value)
        {
            if (type == typeof(IPublishedContent) && Udi.TryParse(value, out var udi))
            {
                return _umbracoContextAccessor.UmbracoContext.Content.GetById(udi);
            }

            return value.TryConvertTo(type).Result;
        }
    }
}