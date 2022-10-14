/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Community.Contentment.Notifications;

namespace Umbraco.Community.Contentment.Composing
{
    internal sealed class ContentmentComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder
                .Services
                    .Configure<ContentmentSettings>(builder.Config.GetSection(Constants.Internals.ConfigurationSection))
             ;

            builder
                .WithCollectionBuilder<ContentmentListItemCollectionBuilder>()
                    .Add(() => builder.TypeLoader.GetTypes<IContentmentListItem>())
            ;

            builder.Services.AddSingleton<ConfigurationEditorUtility>();

            builder
                .AddNotificationHandler<ContentCopyingNotification, ContentBlocksPropertyEditorContentNotificationHandler>()
                .AddNotificationHandler<DataTypeSavedNotification, ContentmentTelemetryNotification>()
                .AddNotificationHandler<ServerVariablesParsingNotification, ContentmentServerVariablesParsingNotification>()
                .AddNotificationHandler<UmbracoApplicationStartingNotification, ContentmentUmbracoApplicationStartingNotification>()
            ;
        }
    }
}
