/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Community.Contentment.Notifications;
using Umbraco.Community.Contentment.Services;

namespace Umbraco.Community.Contentment.Composing
{
    internal sealed class ContentmentComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
#if NET8_0_OR_GREATER == false
            builder
                .ManifestFilters()
                    .Append<ContentmentManifestFilter>()
            ;
#endif

            builder
                .Services
                    .AddSingleton<ConfigurationEditorUtility>()
                    .AddSingleton<IContentmentContentContext, ContentmentContentContext>()
                    .Configure<ContentmentSettings>(builder.Config.GetSection(Constants.Internals.ConfigurationSection))
             ;

            builder
                .WithCollectionBuilder<ContentmentListItemCollectionBuilder>()
                    .Add(() => builder.TypeLoader.GetTypes<IContentmentListItem>())
            ;

            builder
                .WithCollectionBuilder<ContentmentDataListItemPropertyValueConverterCollectionBuilder>()
                    .Add(() => builder.TypeLoader.GetTypes<IDataListItemPropertyValueConverter>())
            ;

            builder
                .AddNotificationAsyncHandler<DataTypeSavedNotification, ContentmentTelemetryNotification>()
                .AddNotificationHandler<ContentCopyingNotification, ContentBlocksPropertyEditorContentNotificationHandler>()
                .AddNotificationHandler<DataTypeDeletedNotification, ContentmentDataTypeNotificationHandler>()
                .AddNotificationHandler<DataTypeSavedNotification, ContentmentDataTypeNotificationHandler>()
                .AddNotificationHandler<ServerVariablesParsingNotification, ContentmentServerVariablesParsingNotification>()
                .AddNotificationHandler<UmbracoApplicationStartingNotification, ContentmentUmbracoApplicationStartingNotification>()
            ;

            builder.Services.Configure<UmbracoPipelineOptions>(opts =>
            {
                opts.AddFilter(new UmbracoPipelineFilter("UmbBlockListGetEmptyByKeysRequest_EnableBuffering")
                {
                    // HACK:  [LK] To support `IContentmentContentContext` inside the BlockList editor,
                    // we need to re-access the `Request.Body` to extract the `parentId` value.
                    // The `GetEmptyByKeys` path has been hard-coded, as unsure how else to generate it at this stage.
                    PrePipeline = app => app.UseWhen(
                        ctx => ctx.Request.Path.StartsWithSegments("/umbraco/backoffice/umbracoapi/content/GetEmptyByKeys"),
                        app2 => app2.Use(async (ctx, next) => { ctx.Request.EnableBuffering(); await next(ctx); })
                    ),
                });
            });
        }
    }
}
