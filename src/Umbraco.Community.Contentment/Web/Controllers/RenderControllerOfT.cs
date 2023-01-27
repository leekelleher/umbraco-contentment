/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472 == false
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;

namespace Umbraco.Cms.Web.Common.Controllers
{
    // NOTE: I'm not sure why Umbraco doesn't have a strongly-typed version of `RenderController`? [LK:2023-01-18]
    [HideFromTypeFinder]
    public abstract class RenderController<T> : RenderController
        where T : class, IPublishedContent
    {
        public RenderController(
            ILogger<RenderController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        { }

        protected new T CurrentPage => base.CurrentPage as T;
    }
}
#endif
