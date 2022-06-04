/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Community.Contentment.DataEditors;
using UmbConstants = Umbraco.Cms.Core.Constants;

namespace Umbraco.Web
{
    public static class PublishedContentTypeExtensions
    {
        public static string GetIcon(this IPublishedContentType contentType, IContentTypeService contentTypeService = null)
        {
            if (contentType != null && ContentTypeCacheHelper.TryGetIcon(contentType.Alias, out var icon, contentTypeService) == true)
            {
                return icon;
            }

            return UmbConstants.Icons.DefaultIcon;
        }
    }
}
