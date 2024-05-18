/* Copyright © 2013-present Umbraco.
 * This Source Code has been derived from Umbraco CMS.
 * https://github.com/umbraco/Umbraco-CMS/blob/release-8.2.0-rc/src/Umbraco.Core/Configuration/UmbracoVersion.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Reflection;
using Umbraco.Cms.Core.Semver;

namespace Umbraco.Community.Contentment
{
    public static class ContentmentVersion
    {
        static ContentmentVersion()
        {
            var assembly = typeof(ContentmentVersion).Assembly;
            var assemblyFileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var semanticVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            AssemblyVersion = assembly.GetName().Version;

            AssemblyFileVersion = assemblyFileVersion is not null ?
                Version.Parse(assemblyFileVersion.Version)
                : AssemblyVersion;

            SemanticVersion = semanticVersion is not null ?
                SemVersion.Parse(semanticVersion.InformationalVersion) :
                AssemblyVersion is not null ? new SemVersion(AssemblyVersion) : default;

            Version = SemanticVersion is not null ?
                new Version(SemanticVersion.Major, SemanticVersion.Minor, SemanticVersion.Patch)
                : AssemblyVersion ?? default;
        }

        public static Version? AssemblyVersion { get; }

        public static Version? AssemblyFileVersion { get; }

        public static string? Comment => SemanticVersion?.Prerelease;

        public static SemVersion? SemanticVersion { get; }

        public static Version? Version { get; }
    }
}
