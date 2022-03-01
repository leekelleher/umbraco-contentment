/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Community.Contentment.Telemetry;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web.Runtime;
#else
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Community.Contentment.Notifications;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.Composing
{
#if NET472
    [ComposeAfter(typeof(WebInitialComposer))]
    [RuntimeLevel(MinLevel = RuntimeLevel.Boot)]
    internal sealed class ContentmentComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition
                .ContentmentListItems()
                    .Add(() => composition.TypeLoader.GetTypes<IContentmentListItem>())
            ;

            composition.RegisterUnique<Core.Hosting.IHostingEnvironment, Polyfill.ContenmentHostingEnvironment>();
            composition.RegisterUnique<Core.IO.IIOHelper, Polyfill.ContenmentIOHelper>();
            composition.RegisterUnique<ConfigurationEditorUtility>();

            if (composition.RuntimeState.Level > RuntimeLevel.Install)
            {
                composition
                    .Components()
                        .Append<ContentmentComponent>()
                ;
            }

            if (composition.RuntimeState.Level == RuntimeLevel.Run)
            {
                if (ContentmentTelemetryComponent.Disabled == false)
                {
                    composition.EnableContentmentTelemetry();
                }
            }
        }
    }
#else
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

            builder.Services.AddUnique<ConfigurationEditorUtility>();

            builder
                .AddNotificationHandler<DataTypeSavedNotification, ContentmentTelemetryNotification>()
                .AddNotificationHandler<ServerVariablesParsingNotification, ContentmentServerVariablesParsingNotification>()
                .AddNotificationHandler<UmbracoApplicationStartingNotification, ContentmentUmbracoApplicationStartingNotification>()
            ;
        }
    }
#endif
}
