/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Xml;

namespace Umbraco.Community.Contentment.Services
{
    public static class ContentmentContentContextExtensions
    {
        public static int? GetCurrentContentId(this IContentmentContentContext ctx) => ctx.GetCurrentContentId(out _);

        public static IPublishedContent? GetCurrentContent(this IContentmentContentContext ctx) => ctx.GetCurrentContent(out _);

        public static string ParseXPathQuery(
            this IContentmentContentContext ctx,
            string xpathExpression,
            Func<int, IEnumerable<string>> getPath,
            Func<int, bool> publishedContentExists)
        {
            var nodeContextId = ctx.GetCurrentContentId(out var isParent);

            if (isParent == true && string.IsNullOrWhiteSpace(xpathExpression) == false)
            {
                xpathExpression = xpathExpression.Replace("$parent", $"id({nodeContextId})");
            }

#pragma warning disable CS0618 // Type or member is obsolete
            return UmbracoXPathPathSyntaxParser.ParseXPathQuery(xpathExpression, nodeContextId, getPath, publishedContentExists);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
