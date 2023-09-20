/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using System;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;

namespace Umbraco.Web.PublishedCache
{
    internal static class PublishedCacheExtensions
    {
        public static IPublishedContentType GetContentType(this IPublishedCache publishedCache, Guid key)
        {
            if (ContentTypeCacheHelper.TryGetAlias(key, out var alias, Current.Services.ContentTypeService) == true)
            {
                return publishedCache.GetContentType(alias);
            }

            return default;
        }
    }
}
#endif
