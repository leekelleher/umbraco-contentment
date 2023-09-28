/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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

        // Backstory on the Umbraco Discord channel:
        // https://discord.com/channels/869656431308189746/882984410432012360/999353549685276702
        public static IUmbracoBuilder AddUmbracoConfiguration(this IUmbracoBuilder builder, Action<IUmbracoBuilder> action)
        {
            action?.Invoke(builder);

            return builder;
        }
    }
}
