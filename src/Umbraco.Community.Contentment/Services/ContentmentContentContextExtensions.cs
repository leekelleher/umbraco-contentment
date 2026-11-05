/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Community.Contentment.Services
{
    public static class ContentmentContentContextExtensions
    {
        public static IPublishedContent? GetCurrentContent(this IContentmentContentContext ctx)
            => ctx.GetCurrentContent(out _);

        public static int? GetCurrentContentId(this IContentmentContentContext ctx)
            => ctx.GetCurrentContentId(out _);

        public static string? GetCurrentContentId<T>(this IContentmentContentContext ctx)
            => ctx.GetCurrentContentId<T>();

        public static Guid? GetCurrentContentTypeKey(this IContentmentContentContext ctx)
            => ctx.GetCurrentContentTypeKey();

        public static string? GetCurrentVariantId(this IContentmentContentContext ctx)
            => ctx.GetCurrentVariantId();

        public static string? GetCurrentCulture(this IContentmentContentContext ctx)
        {
            var variantId = ctx.GetCurrentVariantId();
            if (string.IsNullOrWhiteSpace(variantId) == true || variantId == "invariant")
            {
                return default;
            }

            // `UmbVariantId.toString()` appends an optional "_{segment}" suffix.
            var index = variantId.IndexOf('_');
            return index > 0 ? variantId[..index] : variantId;
        }
    }
}
