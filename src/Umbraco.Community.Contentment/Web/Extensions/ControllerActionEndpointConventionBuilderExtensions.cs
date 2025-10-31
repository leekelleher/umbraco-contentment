/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Cms.Core.Web;

namespace Umbraco.Extensions
{
    public static class ControllerActionEndpointConventionBuilderExtensions
    {
        public static void ForUmbracoPageByDomain(this ControllerActionEndpointConventionBuilder builder)
        {
#pragma warning disable CS8621 // Nullability of reference types in return type doesn't match the target delegate (possibly because of nullability attributes).
            builder.ForUmbracoPage(FindContentByDomain);
#pragma warning restore CS8621 // Nullability of reference types in return type doesn't match the target delegate (possibly because of nullability attributes).
        }

        private static IPublishedContent? FindContentByDomain(ActionExecutingContext actionExecutingContext)
        {
            var accessor = actionExecutingContext.HttpContext.RequestServices.GetRequiredService<IUmbracoContextAccessor>();
            if (accessor?.TryGetUmbracoContext(out var ctx) == true)
            {
                var domain = DomainUtilities.SelectDomain(ctx.Domains?.GetAll(false), ctx.CleanedUmbracoUrl);
                var content = ctx.Content?.GetById(domain?.ContentId ?? -1);

                if (content is not null)
                {
                    return content;
                }

                var navigationService = actionExecutingContext.HttpContext.RequestServices
                    .GetRequiredService<IDocumentNavigationQueryService>();

                navigationService.TryGetRootKeys(out var rootKeys);

                var rootKey = rootKeys.FirstOrDefault();

                if (rootKey == Guid.Empty)
                {
                    return default;
                }

                return ctx.Content?.GetById(rootKey);
            }

            return default;
        }
    }
}
