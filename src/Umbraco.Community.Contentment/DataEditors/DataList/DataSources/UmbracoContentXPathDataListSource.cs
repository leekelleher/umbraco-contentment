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
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
#else
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoContentXPathDataListSource : DataListToDataPickerSourceBridge, IDataListSource, IDataSourceValueConverter
    {
        private readonly IContentmentContentContext _contentmentContentContext;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IIOHelper _ioHelper;

        public UmbracoContentXPathDataListSource(
            IContentmentContentContext contentmentContentContext,
            IUmbracoContextAccessor umbracoContextAccessor,
            IIOHelper ioHelper)
        {
            _contentmentContentContext = contentmentContentContext;
            _umbracoContextAccessor = umbracoContextAccessor;
            _ioHelper = ioHelper;
        }

        public override string Name => "Umbraco Content by XPath";

        public override string Description => "Use an XPath query to select Umbraco content to use as the data source.";

        public override string Icon => "icon-fa fa-file-code-o";

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ConfigurationField
            {
                Key = "xpath",
                Name = "XPath",
                Description = "Enter the XPath expression to select the content.",
                View = "textstring",
            },
            new NotesConfigurationField(_ioHelper, $@"<details class=""well well-small"">
<summary><strong>Do you need help with XPath expressions?</strong></summary>
<p>If you need assistance with XPath syntax in general, please refer to this resource: <a href=""https://developer.mozilla.org/en-US/docs/Web/XPath"" target=""_blank""><strong>MDN Web Docs</strong></a>.</p>
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

        public override Dictionary<string, object> DefaultValues => new Dictionary<string, object>()
        {
            { "xpath", "/root/*[@level = 1]/*[@isDoc]" },
        };

        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var xpath = config.GetValueAs("xpath", string.Empty);

            if (string.IsNullOrWhiteSpace(xpath) == false &&
                _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true &&
                umbracoContext.Content is IPublishedContentCache contentCache)
            {
                var preview = true;

                IEnumerable<string> getPath(int id) => contentCache.GetById(preview, id)?.Path.ToDelimitedList().Reverse();
                bool publishedContentExists(int id) => contentCache.GetById(preview, id) != null;

                var parsed = _contentmentContentContext.ParseXPathQuery(xpath, getPath, publishedContentExists);

                if (string.IsNullOrWhiteSpace(parsed) == false && parsed.StartsWith("$") == false)
                {
                    return contentCache
                        .GetByXPath(preview, parsed)
                        .Select(DataListItemExtensions.ToDataListItem)
                        .ToList();
                }
            }

            return Enumerable.Empty<DataListItem>();
        }

        public Type GetValueType(Dictionary<string, object> config) => typeof(IPublishedContent);

        public object ConvertValue(Type type, string value)
        {
            return UdiParser.TryParse(value, out Udi udi) == true && _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true
                ? umbracoContext.Content.GetById(udi)
                : default;
        }
    }
}
