/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;

namespace Umbraco.Community.Contentment.Services
{
    public sealed class ContentmentContentContext : IContentmentContentContext
    {
        private readonly IRequestAccessor _requestAccessor;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public ContentmentContentContext(
            IRequestAccessor requestAccessor,
            IUmbracoContextAccessor umbracoContextAccessor)
        {
            _requestAccessor = requestAccessor;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public int? GetCurrentContentId(out bool isParent)
        {
            isParent = false;

            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true)
            {
                if (umbracoContext.PublishedRequest?.PublishedContent != null)
                {
                    isParent = false;
                    return umbracoContext.PublishedRequest.PublishedContent.Id;
                }
            }

            // NOTE: First we check for "id" (if on a content page), then "parentId" (if editing an element).
            if (int.TryParse(_requestAccessor.GetRequestValue("id"), out var currentId) == true)
            {
                return currentId;
            }
            else if (int.TryParse(_requestAccessor.GetRequestValue("parentId"), out var parentId) == true)
            {
                isParent = true;

                return parentId;
            }

            return default;
        }

        public IPublishedContent? GetCurrentContent(out bool isParent)
        {
            isParent = false;

            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true)
            {
                if (umbracoContext.PublishedRequest?.PublishedContent != null)
                {
                    return umbracoContext.PublishedRequest.PublishedContent;
                }

                var currentContentId = GetCurrentContentId(out isParent);

                if (currentContentId.HasValue == true)
                {
                    return umbracoContext.Content?.GetById(true, currentContentId.Value);
                }
            }

            return default;
        }
    }
}
