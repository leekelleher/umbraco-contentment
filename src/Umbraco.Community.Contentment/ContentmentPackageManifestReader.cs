// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;
using Umbraco.Community.Contentment.Composing;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment;

internal class ContentmentPackageManifestReader : IPackageManifestReader
{
    private readonly ContentmentSettings _settings;
    private readonly ConfigurationEditorUtility _utility;
    private readonly ContentmentListItemCollection _listItems;

    public ContentmentPackageManifestReader(
        IOptions<ContentmentSettings> settings,
        ConfigurationEditorUtility utility,
        ContentmentListItemCollection listItems)
    {
        _settings = settings.Value;
        _utility = utility;
        _listItems = listItems;
    }

    public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
    {
        var extensions = new List<object>()
        {
            new
            {
                type = "bundle",
                alias = Constants.Internals.ManifestAliasPrefix + "Bundle",
                name = Constants.Internals.ManifestNamePrefix + "Bundle",
                js = Constants.Internals.PackagePathRoot + "manifests.js",
            },
        };

        if (_settings.DisableTree == false)
        {
            extensions.Add(new
            {
                type = "menuItem",
                alias = $"{Constants.Internals.ManifestAliasPrefix}.MenuItem.{Constants.Internals.ProjectName}",
                name = Constants.Internals.ManifestNamePrefix + "Menu Item",
                meta = new
                {
                    label = Constants.Internals.ProjectName,
                    icon = Constants.Icons.Contentment,
                    entityType = Constants.Internals.ProjectAlias,
                    menus = new[] { "Umb.Menu.AdvancedSettings" },
                },
            });
        }

        extensions.AddRange(_listItems.GetExtensionsForManifest(_utility));

        var manifest = new PackageManifest
        {
            Id = Constants.Internals.ProjectNamespace,
            Name = Constants.Internals.ProjectName,
            Version = ContentmentVersion.SemanticVersion?.ToSemanticStringWithoutBuild() ?? "0.0.0",
            AllowTelemetry = true,
            Extensions = extensions.ToArray(),
            Importmap = new PackageManifestImportmap
            {
                Imports = new()
                {
                    { Constants.Internals.ImportMapAlias, Constants.Internals.PackagePathRoot + "index.js" },
                }
            },
        };

        return Task.FromResult(Enumerable.Repeat(manifest, 1));
    }
}
