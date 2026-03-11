/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Services
{
    public sealed class ContentmentContentContext : IContentmentContentContext3
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly IIdKeyMap _idKeyMap;
        private readonly IServiceProvider _serviceProvider;

        public ContentmentContentContext(
            IHttpContextAccessor httpContextAccessor,
            IUmbracoContextAccessor umbracoContextAccessor,
            IVariationContextAccessor variationContextAccessor,
            IIdKeyMap idKeyMap,
            IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _umbracoContextAccessor = umbracoContextAccessor;
            _variationContextAccessor = variationContextAccessor;
            _idKeyMap = idKeyMap;
            _serviceProvider = serviceProvider;
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
                    SetVariationContext();
                    return umbracoContext.Content?.GetById(true, currentContentId.Value);
                }

                // NOTE: Next we check for a GUID ID.
                var currentContentKey = GetCurrentContentId<Guid?>(out isParent);
                if (currentContentKey.HasValue == true)
                {
                    SetVariationContext();

                    var content = umbracoContext.Content?.GetById(true, currentContentKey.Value);

                    // NOTE: Check if the content is a member.
                    if (content is null)
                    {
                        var memberKeyAttempt = _idKeyMap.GetIdForKey(currentContentKey.Value, UmbracoObjectTypes.Member);
                        if (memberKeyAttempt.Success)
                        {
                            using var scope = _serviceProvider.CreateScope();
                            var memberManager = scope.ServiceProvider.GetRequiredService<IMemberManager>();

                            var memberId = memberKeyAttempt.Result;
                            var member = memberManager.FindByIdAsync(memberId.ToString()).Result;
                            if (member is not null)
                            {
                                content = memberManager.AsPublishedMember(member);
                            }
                        }
                    }
                }
            }

            return default;
        }

        public string? GetCurrentVariantId()
        {
            if (_httpContextAccessor.HttpContext?.Items.TryGetValueAs("contentmentContextCurrentContentVariantId", out string? variantId) == true &&
                string.IsNullOrWhiteSpace(variantId) == false)
            {
                return variantId;
            }

            return default;
        }

        private void SetVariationContext()
        {
            var variantId = GetCurrentVariantId();
            if (string.IsNullOrWhiteSpace(variantId) == false && variantId != "invariant")
            {
                _variationContextAccessor.VariationContext = new VariationContext(variantId);
            }
        }
    }
}
