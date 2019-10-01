/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Umbraco.Community.Contentment
{
    internal static partial class Constants
    {
        internal static partial class Internals
        {
            internal const string ProjectName = "Contentment";

            internal const string ProjectNamespace = "Umbraco.Community.Contentment";

            internal const string DataEditorNamePrefix = "[" + ProjectName + "] ";

            internal const string DataEditorAliasPrefix = ProjectNamespace + ".";

            internal const string EditorsPathRoot = PackagePathRoot + "editors/";

            internal const string PackagePathRoot = "~/App_Plugins/" + ProjectName + "/";

            internal const string PluginControllerName = ProjectName;

            internal const string TreeAlias = "contentment";
        }

        internal static partial class Conventions
        {
            internal static partial class PropertyGroups
            {
                public const string Code = "Code";

                public const string Display = "Display";
            }
        }

        internal static partial class Package
        {
            internal const string Author = "Lee Kelleher";

            internal const string AuthorUrl = "https://leekelleher.com";

            internal static readonly System.Version ContentmentVersion = new System.Version(1, 0, 0);

            internal const string IconUrl = "https://raw.githubusercontent.com/leekelleher/umbraco-contentment/master/docs/assets/img/logo.png";

            internal const string License = "Mozilla Public License";

            internal const string LicenseUrl = "https://mozilla.org/MPL/2.0/";

            internal static readonly System.Version MinimumSupportedUmbracoVersion = new System.Version(8, 1, 0);

            internal const string RepositoryUrl = "https://github.com/leekelleher/umbraco-contentment";
        }

        internal static partial class Values
        {
            public const string True = "1";

            public const string False = "0";
        }
    }
}
