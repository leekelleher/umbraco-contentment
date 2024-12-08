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
            internal const string ProjectName = nameof(Contentment);

            internal const string ProjectAlias = "contentment";

            internal const string ProjectNamespace = $"{nameof(Umbraco)}.{nameof(Community)}.{nameof(Contentment)}";

            internal const string DataEditorNamePrefix = $"[{ProjectName}] ";

            internal const string DataEditorAliasPrefix = $"{ProjectNamespace}.";

            internal const string DataEditorUiAliasPrefix = $"Umb.{ProjectName}.PropertyEditorUi.";

            [Obsolete("To be removed in Contentment 7.0")]
            internal const string EditorsPathRoot = $"{PackagePathRoot}editors/";

            internal const string PackagePathRoot = $"{UmbConstants.SystemDirectories.AppPlugins}/{ProjectName}/";

            internal const string PluginControllerName = ProjectName;

            [Obsolete("To be removed in Contentment 7.0")]
            internal const string BackOfficePathRoot = $"{PackagePathRoot}backoffice/{TreeAlias}/";

            [Obsolete("To be removed in Contentment 7.0")]
            internal const string TreeAlias = ProjectAlias;

            internal const string ConfigurationSection = $"{nameof(Umbraco)}:{nameof(Contentment)}";

            [Obsolete("To be removed in Contentment 7.0")]
            internal const string EmptyEditorViewPath = $"{EditorsPathRoot}_empty.html";

            public const string RepositoryUrl = "https://github.com/leekelleher/umbraco-contentment";
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
                [Obsolete("To be removed in Contentment 7.0")]
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

        internal static partial class Persistance
        {
            internal static partial class Providers
            {
                public const string Sqlite = "Microsoft.Data.Sqlite";

                public const string SqlServer = "Microsoft.Data.SqlClient";
            }
        }

        internal static partial class Icons
        {
            public const string Contentment = $"icon-{Internals.ProjectAlias}";

            public const string ContentTemplate = "icon-blueprint";
        }

        // TODO: [LK:2024-12-06] Figure out if this is still needed?
        internal static partial class Values
        {
            public const bool True = true;

            public const bool False = false;
        }
    }
}
