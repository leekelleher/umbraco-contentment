/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Manifest;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment;

internal sealed class ContentmentManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {
        manifests.Add(new PackageManifest
        {
            AllowPackageTelemetry = true,
            PackageName = Constants.Internals.ProjectName,
            Scripts = new[] { Constants.Internals.PackagePathRoot + Constants.Internals.ProjectAlias + ".js" },
            Stylesheets = new[] { Constants.Internals.PackagePathRoot + Constants.Internals.ProjectAlias + ".css" },
            Version = ContentmentVersion.SemanticVersion?.ToSemanticStringWithoutBuild() ?? "5.0.0",
        });
    }
}
