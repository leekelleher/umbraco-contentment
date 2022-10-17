/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Core.Models.PublishedContent;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
#endif

namespace Umbraco.Community.Contentment.Services
{
    public static class ContentmentContentContextExtensions
    {
        public static int? GetCurrentContentId(this IContentmentContentContext ctx) => ctx.GetCurrentContentId(out _);

        public static IPublishedContent GetCurrentContent(this IContentmentContentContext ctx) => ctx.GetCurrentContent(out _);
    }
}
