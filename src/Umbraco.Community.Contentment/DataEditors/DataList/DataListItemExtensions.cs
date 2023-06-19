/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Web;
#if NET472
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal static class DataListItemExtensions
    {
        internal static DataListItem ToDataListItem(
            this IPublishedContent content,
            string imageAlias = "image",
            IContentTypeService contentTypeService = null)
        {
            return new DataListItem
            {
                Name = content.Name,
                Description = content.TemplateId > 0 ? content.Url() : string.Empty,
                Disabled = content.IsPublished() == false,
                Icon = content.ContentType.GetIcon(contentTypeService),
                Properties = new Dictionary<string, object>
                {
                    { "image", content.Value<IPublishedContent>(imageAlias)?.Url() },
                },
                Value = content.GetUdi().ToString(),
            };
        }

        internal static DataListItem ToDataListItem(this SocialLink link)
        {
            return new DataListItem
            {
                Name = link.Name,
                Description = link.Url,
                Icon = $"icon-{link.Network}",
                Value = link.Network,
            };
        }
    }
}
