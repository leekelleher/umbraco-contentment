/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Infrastructure.Manifest;
using Umbraco.Community.Contentment.Api.Management;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Community.Contentment.Notifications;
using Umbraco.Community.Contentment.Services;

namespace Umbraco.Community.Contentment.Composing
{
    public sealed class ContentmentComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder
                .Services
                    .AddSingleton<ConfigurationEditorUtility>()
                    .AddSingleton<IContentmentContentContext, ContentmentContentContext>()
                    .AddSingleton<IOperationIdHandler, ContentmentOperationIdHandler>()
                    .AddSingleton<IPackageManifestReader, ContentmentPackageManifestReader>()
                    .Configure<ContentmentSettings>(builder.Config.GetSection(Constants.Internals.ConfigurationSection))
                    .ConfigureOptions<ConfigureContentmentSwaggerGenOptions>()
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
            ;
        }
    }
}
