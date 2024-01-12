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
            PackageId = Constants.Internals.ProjectNamespace,
            PackageName = Constants.Internals.ProjectName,
            Scripts = [$"{Constants.Internals.PackagePathRoot}{Constants.Internals.ProjectAlias}.js"],
            Stylesheets = [$"{Constants.Internals.PackagePathRoot}{Constants.Internals.ProjectAlias}.css"],
            Version = ContentmentVersion.SemanticVersion?.ToSemanticStringWithoutBuild() ?? "0.0.0",
        });
    }
}
