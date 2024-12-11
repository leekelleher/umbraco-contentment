// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Manifest;
using Umbraco.Community.Contentment.Composing;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment;

internal class ContentmentPackageManifestReader : IPackageManifestReader
{
    private readonly ConfigurationEditorUtility _utility;
    private readonly IShortStringHelper _shortStringHelper;

    public ContentmentPackageManifestReader(
        ConfigurationEditorUtility utility,
        IShortStringHelper shortStringHelper)
    {
        _utility = utility;
        _shortStringHelper = shortStringHelper;
    }

    public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
    {
        var extensions = new List<object>()
        {
            new
            {
                type = "bundle",
                alias = $"Umb.{Constants.Internals.ProjectName}.Bundle",
                name = Constants.Internals.DataEditorNamePrefix + "Bundle",
                js = $"{Constants.Internals.PackagePathRoot}umbraco-{Constants.Internals.ProjectAlias}.js"
            },
        };
        var manifest = new PackageManifest
        {
            Id = Constants.Internals.ProjectNamespace,
            Name = Constants.Internals.ProjectName,
            Version = ContentmentVersion.SemanticVersion?.ToSemanticStringWithoutBuild() ?? "0.0.0",
            AllowTelemetry = true,
            Extensions = extensions.ToArray(),
        };

        return Task.FromResult(manifest.AsEnumerableOfOne());
    }
}
