/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Community.Contentment.DataEditors;

namespace Umbraco.Extensions
{
    public static class PublishedContentTypeExtensions
    {
        private static IContentTypeService _contentTypeService => StaticServiceProvider.Instance.GetRequiredService<IContentTypeService>();

        public static string GetIcon(this IPublishedContentType contentType, IContentTypeService? contentTypeService = null)
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
