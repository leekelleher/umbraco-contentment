/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Community.Contentment.Trees;
using Umbraco.Extensions;
using Umbraco.Web;

// NOTE: This extension method class is deliberately using the Umbraco namespace,
// as to reduce namespace imports and ease the developer experience. [LK]
namespace Umbraco.Core.Composing
{
    public static partial class CompositionExtensions
    {
        public static IUmbracoBuilder DisableContentmentTree(this IUmbracoBuilder builder)
        {
            // TODO: [LK:2021-05-03] Commented out, as I had to comment out `ContentmentTreeController` for now.

            //builder
            //    .Trees()
            //        .RemoveTreeController<ContentmentTreeController>()
            //;

            return builder;
        }
    }
}
