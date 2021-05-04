/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.WebAssets;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Core.Composing;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Composing
{
    // TODO: [LK:2021-04-03] v9 Review this.
    //[ComposeAfter(typeof(WebInitialComposer))]
    internal sealed class ContentmentComposer : IUserComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder
                .ContentmentListItems()
                    .Add(() => builder.TypeLoader.GetTypes<IContentmentListItem>())
            ;

            builder.Services.AddUnique<ConfigurationEditorUtility>();

            //if (_runtimeState.Level > RuntimeLevel.Install)
            {
                builder
                    .Components()
                        .Append<ContentmentComponent>()
                ;

                builder.AddNotificationHandler<ServerVariablesParsing, ContentmentServerVariablesParsing>();
            }

            //if (_runtimeState.Level == RuntimeLevel.Run)
            {
                //    if (ContentmentTelemetryComponent.Disabled == false)
                //    {
                //        builder.EnableContentmentTelemetry();
                //    }
            }
        }
    }
}
