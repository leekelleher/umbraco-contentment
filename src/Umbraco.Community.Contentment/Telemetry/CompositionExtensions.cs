/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Community.Contentment.Telemetry;

// NOTE: This extension method class is deliberately using the Umbraco namespace,
// as to reduce namespace imports and ease the developer experience. [LK]
namespace Umbraco.Core.Composing
{
    public static partial class CompositionExtensions
    {
        public static IUmbracoBuilder EnableContentmentTelemetry(this IUmbracoBuilder builder)
        {
            ContentmentTelemetryComponent.Disabled = false;

            // TODO: [LK:2021-04-30] v9 Review this.
            //builder
            //    .Components()
            //        .Append<ContentmentTelemetryComponent>()
            //;

            // TODO: [LK:2021-04-30] v9 Maybe renamed this to `ContentmentTelemetryHandler`
            builder.AddNotificationHandler<SavedNotification<IDataType>, ContentmentTelemetryComponent>();

            return builder;
        }

        public static IUmbracoBuilder DisableContentmentTelemetry(this IUmbracoBuilder builder)
        {
            ContentmentTelemetryComponent.Disabled = true;

            // TODO: [LK:2021-04-30] v9 Review this.
            //builder
            //    .Components()
            //        .Remove<ContentmentTelemetryComponent>()
            //;

            return builder;
        }
    }
}
