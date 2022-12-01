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

            internal const string ConfigurationSection = "Umbraco:Contentment";

            internal const string EmptyEditorViewPath = EditorsPathRoot + "_empty.html";
        }

        internal static partial class Conventions
        {
            internal static partial class ConfigurationFieldAliases
            {
                public const string AddButtonLabelKey = "addButtonLabelKey";

                public const string DefaultValue = "defaultValue";

                public const string Items = "items";

                public const string OverlayView = "overlayView";
            }

            internal static partial class DataSourceGroups
            {
                public const string Data = nameof(Data);

                public const string DotNet = ".NET";

                public const string Umbraco = nameof(Umbraco);

                public const string Web = nameof(Web);
            }

            internal static partial class DefaultConfiguration
            {
                public static readonly object RichTextEditor = new
                {
                    maxImageSize = 500,
                    mode = "classic",
                    stylesheets = false,
                    toolbar = new[]
                    {
                        "ace",
                        "undo",
                        "redo",
                        "cut",
                        "styleselect",
                        "removeformat",
                        "bold",
                        "italic",
                        "alignleft",
                        "aligncenter",
                        "alignright",
                        "bullist",
                        "numlist",
                        "link",
                        "umbmediapicker",
                        "umbmacro",
                        "umbembeddialog"
                    },
                };
            }

            internal static partial class PropertyGroups
            {
                public const string Code = nameof(Code);

                public const string Display = nameof(Display);
            }
        }

        internal static partial class Icons
        {
            public const string Contentment = "icon-contentment";

            public const string ContentTemplate = "icon-blueprint";
        }

        internal static partial class Package
        {
            public const string Author = "Lee Kelleher";

            public const string AuthorUrl = "https://leekelleher.com";

            public const string IconUrl = "https://raw.githubusercontent.com/leekelleher/umbraco-contentment/master/docs/assets/img/logo.png";

            public const string License = "Mozilla Public License";

            public const string LicenseUrl = "https://mozilla.org/MPL/2.0/";

#if NET472
            public static readonly System.Version MinimumSupportedUmbracoVersion = new System.Version(8, 17, 0);
#elif NET5_0
            public static readonly System.Version MinimumSupportedUmbracoVersion = new System.Version(9, 5, 0);
#elif NET6_0
            public static readonly System.Version MinimumSupportedUmbracoVersion = new System.Version(10, 0, 0);
#else
            public static readonly System.Version MinimumSupportedUmbracoVersion = new System.Version(11, 0, 0);
#endif

            public const string RepositoryUrl = "https://github.com/leekelleher/umbraco-contentment";
        }

        internal static partial class Values
        {
            public const string True = "1";

            public const string False = "0";
        }
    }
}
