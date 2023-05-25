/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
#endif

namespace Umbraco.Community.Contentment.Services
{
    public sealed class ContentmentContentContext : IContentmentContentContext
    {
#if NET472 == false
        private readonly IRequestAccessor _requestAccessor;
#endif
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public ContentmentContentContext(
#if NET472 == false
            IRequestAccessor requestAccessor,
#endif
            IUmbracoContextAccessor umbracoContextAccessor)
        {
#if NET472 == false
            _requestAccessor = requestAccessor;
#endif
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

#if NET472 == false
            }
#endif

                // NOTE: First we check for "id" (if on a content page), then "parentId" (if editing an element).
#if NET472
                if (int.TryParse(umbracoContext.HttpContext.Request.QueryString.Get("id"), out var currentId) == true)
#else
                if (int.TryParse(_requestAccessor.GetQueryStringValue("id"), out var currentId) == true)
#endif
                {
                    return currentId;
                }
#if NET472
                else if (int.TryParse(umbracoContext.HttpContext.Request.QueryString.Get("parentId"), out var parentId) == true)
#else
                else if (int.TryParse(_requestAccessor.GetQueryStringValue("parentId"), out var parentId) == true)
#endif
                {
                    isParent = true;

                    return parentId;
                }
#if NET472
            }
#endif
            return default;
        }

        public IPublishedContent GetCurrentContent(out bool isParent)
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
                    return umbracoContext.Content.GetById(true, currentContentId.Value);
                }
            }

            return default;
        }
    }
}
