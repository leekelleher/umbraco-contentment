using System;
using System.Collections.Generic;
using Umbraco.Community.Contentment.Services;
#if NET472
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Xml;
#else
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Xml;
using Umbraco.Cms.Web.Common.DependencyInjection;
#endif

namespace Umbraco.Community.Contentment.Xml
{
    internal class ContentmentXPathSyntaxParser : UmbracoXPathPathSyntaxParser
    {
        public static string ParseXPathQuery(string xpathExpression, Func<int, IEnumerable<string>> getPath, Func<int, bool> publishedContentExists)
        {
#if NET472
            var contentmentContentContext = Current.Factory.GetInstance<IContentmentContentContext>();
#else
            var contentmentContentContext = StaticServiceProvider.Instance.GetService<IContentmentContentContext>();
#endif
            var nodeContextId = contentmentContentContext.GetCurrentContentId(out var isParent);

            if (isParent == true)
            {
                xpathExpression = xpathExpression?.Replace("$parent", $"id({nodeContextId})");
            }

            return ParseXPathQuery(xpathExpression, nodeContextId, getPath, publishedContentExists);
        }
    }
}
