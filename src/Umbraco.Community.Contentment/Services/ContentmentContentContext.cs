/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Services
{
    public sealed class ContentmentContentContext : IContentmentContentContext2
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

            // TODO: [LK:2024-12-06] Figure out if this is still needed?
            var json = _httpContextAccessor.HttpContext?.Request.GetRawBodyStringAsync().GetAwaiter().GetResult();
            if (string.IsNullOrWhiteSpace(json) == false)
            {
                var obj = DeserializeAnonymousType(json, new { id = (object?)null, parentId = (object?)null });
                if (obj is not null && obj.id is not null)
                {
                    return obj.id.TryConvertTo<T>().Result;
                }
                else if (obj is not null && obj.parentId is not null)
                {
                    isParent = true;
                    return obj.parentId.TryConvertTo<T>().Result;
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

        private T? DeserializeAnonymousType<T>(string json, T anonymousObj) => _jsonSerializer.Deserialize<T>(json);
    }
}
