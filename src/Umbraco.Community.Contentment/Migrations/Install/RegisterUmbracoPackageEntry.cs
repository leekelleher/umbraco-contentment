/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using System.Linq;
using Umbraco.Core.Migrations;
using Umbraco.Core.Models.Packaging;
using Umbraco.Core.Services;
#else
using Umbraco.Cms.Infrastructure.Migrations;
#endif

namespace Umbraco.Community.Contentment.Migrations
{
#if NET472
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
                    Version = ContentmentVersion.Version.ToString(),
                    Readme = "",
                });
            }
        }
    }
#else
    internal sealed class RegisterUmbracoPackageEntry : MigrationBase
    {
        public const string State = "{contentment-reg-umb-pkg-entry}";

        public RegisterUmbracoPackageEntry(IMigrationContext context)
            : base(context)
        { }

        protected override void Migrate()
        {
            // NOTE: This migration does nothing. It is a left over from code targeting Umbraco 8.
            // It needs to remain, as Umbraco instances that have been upgraded from v8 to v9 will have reached this migrated state.
        }
    }
#endif
}
