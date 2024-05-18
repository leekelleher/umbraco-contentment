/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Extensions
{
    public static class PublishedContentExtensions
    {
        public static Link ToLink<TModel>(this TModel model, LinkType linkType = LinkType.Content)
              where TModel : IPublishedContent
        {
            return new Link
            {
                Name = model.Name,
                Type = linkType,
                Udi = model.GetUdi(),
                Url = model.Url(),
            };
        }
    }
}
