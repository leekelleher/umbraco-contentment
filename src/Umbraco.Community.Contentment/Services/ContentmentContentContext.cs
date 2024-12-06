/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Services
{
    public sealed class ContentmentContentContext : IContentmentContentContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IRequestAccessor _requestAccessor;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public ContentmentContentContext(
            IHttpContextAccessor httpContextAccessor,
            IJsonSerializer jsonSerializer,
            IRequestAccessor requestAccessor,
            IUmbracoContextAccessor umbracoContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _jsonSerializer = jsonSerializer;
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

            // TODO: [LK:2024-12-06] Figure out if this is still needed?
            var json = _httpContextAccessor.HttpContext?.Request.GetRawBodyStringAsync().GetAwaiter().GetResult();
            if (string.IsNullOrWhiteSpace(json) == false)
            {
                var obj = DeserializeAnonymousType(json, new { id = 0, parentId = 0 });
                if (obj?.id > 0)
                {
                    return obj.id;
                }
                else if (obj?.parentId > 0)
                {
                    isParent = true;
                    return obj.parentId;
                }
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

        private T? DeserializeAnonymousType<T>(string json, T anonymousObj) => _jsonSerializer.Deserialize<T>(json);
    }
}
