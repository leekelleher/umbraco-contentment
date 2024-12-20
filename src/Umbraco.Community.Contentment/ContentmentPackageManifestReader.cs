// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using System.Xml.Linq;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Core.Strings;
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
    private readonly IShortStringHelper _shortStringHelper;

    public ContentmentPackageManifestReader(
        IOptions<ContentmentSettings> settings,
        ConfigurationEditorUtility utility,
        ContentmentListItemCollection listItems,
        IShortStringHelper shortStringHelper)
    {
        _settings = settings.Value;
        _utility = utility;
        _listItems = listItems;
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
                name = Constants.Internals.ManifestNamePrefix + "Bundle",
                js = $"{Constants.Internals.PackagePathRoot}umbraco-{Constants.Internals.ProjectAlias}.js"
            },
        };

        if (_settings.DisableTree == false)
        {
            extensions.Add(new
            {
                type = "menuItem",
                alias = $"Umb.{Constants.Internals.ProjectName}.MenuItem.{Constants.Internals.ProjectName}",
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

        extensions.AddRange(_listItems.GetExtensionsForManifest(_utility, _shortStringHelper));

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
