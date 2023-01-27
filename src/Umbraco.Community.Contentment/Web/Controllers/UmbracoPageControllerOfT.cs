/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472 == false
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Cms.Web.Common.Controllers
{
    // NOTE: I'm not sure why Umbraco doesn't have a strongly-typed version of `UmbracoPageController`? [LK:2023-01-18]
    [HideFromTypeFinder]
    public abstract class UmbracoPageController<T> : UmbracoPageController
        where T : class, IPublishedContent
    {
        protected UmbracoPageController(
            ILogger<UmbracoPageController> logger,
            ICompositeViewEngine compositeViewEngine)
            : base(logger, compositeViewEngine)
        { }

        protected new T CurrentPage => base.CurrentPage as T;
    }
}
#endif
