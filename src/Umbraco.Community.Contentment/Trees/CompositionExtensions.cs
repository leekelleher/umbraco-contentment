/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web;
using Umbraco.Community.Contentment.Trees;
using Umbraco.Extensions;

namespace Umbraco.Core.Composing
{
    public static partial class CompositionExtensions
    {
        public static IUmbracoBuilder DisableContentmentTree(this IUmbracoBuilder builder)
        {
            builder
                .Trees()
                    .RemoveTreeController<ContentmentTreeController>()
            ;

            return builder;
        }
    }
}
