/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Linq;
using Umbraco.Cms.Core.Packaging;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco.Community.Contentment.Migrations
{
    internal sealed class RegisterUmbracoPackageEntry : MigrationBase
    {
        private readonly IPackagingService _packagingService;

        public const string State = "{contentment-reg-umb-pkg-entry}";

        public RegisterUmbracoPackageEntry(IPackagingService packagingService, IMigrationContext context)
            : base(context)
        {
            _packagingService = packagingService;
        }

        public override void Migrate()
        {
            // Check if the package has already been installed.
            var pkgs = _packagingService.GetInstalledPackageByName(Constants.Internals.ProjectName);
            if (pkgs?.Any() == false)
            {
                // If not, then make a package definition and save it to the "installedPackages.config".
                _packagingService.SaveInstalledPackage(new PackageDefinition
                {
                    Name = Constants.Internals.ProjectName,
                    Url = Constants.Package.RepositoryUrl,
                    Author = Constants.Package.Author,
                    AuthorUrl = Constants.Package.AuthorUrl,
                    IconUrl = Constants.Package.IconUrl,
                    License = Constants.Package.License,
                    LicenseUrl = Constants.Package.LicenseUrl,
                    UmbracoVersion = Constants.Package.MinimumSupportedUmbracoVersion,
                    Version = Configuration.ContentmentVersion.Version.ToString(),
                    Readme = "",
                });
            }
        }
    }
}
