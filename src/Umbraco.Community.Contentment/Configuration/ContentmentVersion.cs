/* Copyright © 2013-present Umbraco.
 * This Source Code has been derived from Umbraco CMS.
 * https://github.com/umbraco/Umbraco-CMS/blob/release-8.2.0-rc/src/Umbraco.Core/Configuration/UmbracoVersion.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Reflection;
using Semver;
using Umbraco.Core;

namespace Umbraco.Community.Contentment.Configuration
{
    public static class ContentmentVersion
    {
        static ContentmentVersion()
        {
            var assembly = typeof(ContentmentVersion).Assembly;

            AssemblyVersion = assembly.GetName().Version;

            AssemblyFileVersion = Version.Parse(assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);

            SemanticVersion = SemVersion.Parse(assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);

            Version = new Version(SemanticVersion.Major, SemanticVersion.Minor, SemanticVersion.Patch);
        }

        public static Version AssemblyVersion { get; }

        public static Version AssemblyFileVersion { get; }

        public static string Comment => SemanticVersion.Prerelease;

        public static SemVersion SemanticVersion { get; }

        public static Version Version { get; }
    }
}
