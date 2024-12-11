/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Services
{
    public sealed class ContentmentContentContext : IContentmentContentContext2
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestAccessor _requestAccessor;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public ContentmentContentContext(
            IHttpContextAccessor httpContextAccessor,
            IRequestAccessor requestAccessor,
            IUmbracoContextAccessor umbracoContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _requestAccessor = requestAccessor;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public T? GetCurrentContentId<T>(out bool isParent)
        {
            isParent = false;

            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true &&
                umbracoContext.PublishedRequest?.PublishedContent is not null)
            {
                var attempt = umbracoContext.PublishedRequest.PublishedContent.Id.TryConvertTo<T>();
                if (attempt.Success)
                {
                    return attempt.Result;
                }
            }

            // NOTE: First we check for "id" (if on a content page), then "parentId" (if editing an element).
            var attempt1 = _requestAccessor.GetRequestValue("id")?.TryConvertTo<T>() ?? Attempt.Fail<T>();
            if (attempt1.Success)
            {
                return attempt1.Result;
            }

            var attempt2 = _requestAccessor.GetRequestValue("parentId")?.TryConvertTo<T>() ?? Attempt.Fail<T>();
            if (attempt2.Success)
            {
                isParent = true;
                return attempt2.Result;
            }

            var json = _httpContextAccessor.HttpContext?.Request.GetRawBodyStringAsync().GetAwaiter().GetResult();
            if (string.IsNullOrWhiteSpace(json) == false)
            {
                var obj = JsonConvert.DeserializeAnonymousType(json, new { id = default(T), parentId = default(T) });
                if (obj is not null && obj.id is not null)
                {
                    return obj.id;
                }
                else if (obj is not null && obj.parentId is not null)
                {
                    isParent = true;
                    return obj.parentId;
                }
            }

            return default;
        }

        public int? GetCurrentContentId(out bool isParent) => GetCurrentContentId<int>(out isParent);

        public IPublishedContent? GetCurrentContent(out bool isParent)
        {
            isParent = false;

            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true)
            {
                if (umbracoContext.PublishedRequest?.PublishedContent != null)
                {
                    return umbracoContext.PublishedRequest.PublishedContent;
                }

                // NOTE: First we check for an integer ID.
                var currentContentId = GetCurrentContentId<int?>(out isParent);

                if (currentContentId.HasValue == true)
                {
                    return umbracoContext.Content?.GetById(true, currentContentId.Value);
                }

                // NOTE: Next we check for a GUID ID.
                var currentContentKey = GetCurrentContentId<Guid?>(out isParent);

                if (currentContentKey.HasValue == true)
                {
                    return umbracoContext.Content?.GetById(true, currentContentKey.Value);
                }
            }

            return default;
        }
    }
}
