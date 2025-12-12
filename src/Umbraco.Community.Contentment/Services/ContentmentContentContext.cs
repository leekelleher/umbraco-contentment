/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Services
{
    public sealed class ContentmentContentContext : IContentmentContentContext2
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        [Obsolete("Use constructor without the `IJsonSerializer` and `IRequestAccessor` parameters. This constructor will be removed in Contentment 8.0.")]
        public ContentmentContentContext(
            IHttpContextAccessor httpContextAccessor,
            IJsonSerializer jsonSerializer,
            IRequestAccessor requestAccessor,
            IUmbracoContextAccessor umbracoContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public ContentmentContentContext(
            IHttpContextAccessor httpContextAccessor,
            IUmbracoContextAccessor umbracoContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public T? GetCurrentContentId<T>(out bool isParent)
        {
            isParent = false;

            if (_httpContextAccessor.HttpContext?.Items.TryGetValueAs("contentmentContextCurrentContentId", out T? unique) == true &&
                unique is not null)
            {
                return unique;
            }

            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true &&
                umbracoContext.PublishedRequest?.PublishedContent is not null)
            {
                var attempt = umbracoContext.PublishedRequest.PublishedContent.Id.TryConvertTo<T>();
                if (attempt.Success)
                {
                    return attempt.Result;
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
                // Cursory check if this is being called during a published/routable request.
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
