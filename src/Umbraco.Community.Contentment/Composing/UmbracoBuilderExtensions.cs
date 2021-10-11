/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472 == false
using System;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Community.Contentment;
using Umbraco.Community.Contentment.Composing;

namespace Umbraco.Extensions
{
    public static partial class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddContentment(
            this IUmbracoBuilder builder,
            Action<ContentmentSettings> settings = default,
            Action<ContentmentListItemCollectionBuilder> listItems = default)
        {
            if (settings is not null)
            {
                _ = builder.Services.PostConfigure(settings);
            }

            if (listItems is not null)
            {
                listItems(builder.WithCollectionBuilder<ContentmentListItemCollectionBuilder>());
            }

            return builder;
        }
    }
}
#endif
