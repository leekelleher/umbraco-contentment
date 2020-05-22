/* Copyright © 2019 Lee Kelleher.
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

            internal const string ProjectAlias = "contentment";

            internal const string ProjectNamespace = "Umbraco.Community.Contentment";

            internal const string DataEditorNamePrefix = "[" + ProjectName + "] ";

            internal const string DataEditorAliasPrefix = ProjectNamespace + ".";

            internal const string EditorsPathRoot = PackagePathRoot + "editors/";

            internal const string PackagePathRoot = "~/App_Plugins/" + ProjectName + "/";

            internal const string PluginControllerName = ProjectName;

            internal const string BackOfficePathRoot = PackagePathRoot + "backoffice/" + TreeAlias + "/";

            internal const string TreeAlias = ProjectAlias;
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
            public const string Author = "Lee Kelleher";

            public const string AuthorUrl = "https://leekelleher.com";

            public const string IconUrl = "https://raw.githubusercontent.com/leekelleher/umbraco-contentment/master/docs/assets/img/logo.png";

            public const string License = "Mozilla Public License";

            public const string LicenseUrl = "https://mozilla.org/MPL/2.0/";

            public static readonly System.Version MinimumSupportedUmbracoVersion = new System.Version(8, 6, 1);

            public const string RepositoryUrl = "https://github.com/leekelleher/umbraco-contentment";
        }

        internal static partial class Values
        {
            public const string True = "1";

            public const string False = "0";
        }
    }
}
