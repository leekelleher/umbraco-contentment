/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472 == false
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;

namespace Umbraco.Extensions
{
    public static class ControllerActionEndpointConventionBuilderExtensions
    {
        public static void ForUmbracoPageByDomain(this ControllerActionEndpointConventionBuilder builder)
        {
            builder.ForUmbracoPage(FindContentByDomain);
        }

        private static IPublishedContent FindContentByDomain(ActionExecutingContext actionExecutingContext)
        {
            var accessor = actionExecutingContext.HttpContext.RequestServices.GetRequiredService<IUmbracoContextAccessor>();
            if (accessor?.TryGetUmbracoContext(out var ctx) == true)
            {
                var domain = DomainUtilities.SelectDomain(ctx.Domains?.GetAll(false), ctx.CleanedUmbracoUrl);
                var content = ctx.Content?.GetById(domain?.ContentId ?? -1);

                return content ?? ctx.Content?.GetAtRoot().FirstOrDefault();
            }

            return null;
        }
    }
}
#endif
