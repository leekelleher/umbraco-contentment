/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using UmbConstants = Umbraco.Core.Constants;

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
