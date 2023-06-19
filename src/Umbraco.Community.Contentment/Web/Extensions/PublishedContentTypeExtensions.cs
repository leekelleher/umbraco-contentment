/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Community.Contentment.DataEditors;
#if NET472
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using UmbConstants = Umbraco.Core.Constants;
#else
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.DependencyInjection;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Web
{
    public static class PublishedContentTypeExtensions
    {
#if NET472
        private static IContentTypeService _contentTypeService => Current.Services.ContentTypeService;
#else
        private static IContentTypeService _contentTypeService => StaticServiceProvider.Instance.GetRequiredService<IContentTypeService>();
#endif

        public static string GetIcon(this IPublishedContentType contentType, IContentTypeService contentTypeService = null)
        {
            if (contentType != null &&
                ContentTypeCacheHelper.TryGetIcon(contentType.Alias, out var icon, contentTypeService ?? _contentTypeService) == true)
            {
                return icon;
            }

            return UmbConstants.Icons.DefaultIcon;
        }
    }
}
