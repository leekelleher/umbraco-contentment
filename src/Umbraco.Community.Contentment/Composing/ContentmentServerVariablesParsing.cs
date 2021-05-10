/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Infrastructure.WebAssets;
using Umbraco.Cms.Core;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Composing
{
    internal sealed class ContentmentServerVariablesParsing : INotificationHandler<ServerVariablesParsing>
    {
        public void Handle(ServerVariablesParsing notification)
        {
            if (notification.ServerVariables.TryGetValueAs("umbracoPlugins", out Dictionary<string, object> umbracoPlugins) == true &&
                umbracoPlugins.ContainsKey(Constants.Internals.ProjectAlias) == false)
            {
                umbracoPlugins.Add(Constants.Internals.ProjectAlias, new
                {
                    name = Constants.Internals.ProjectName,
                    version = Configuration.ContentmentVersion.SemanticVersion.ToSemanticString(),
                    telemetry = Telemetry.ContentmentTelemetryComponent.Disabled == false,
                });
            }
        }
    }
}
