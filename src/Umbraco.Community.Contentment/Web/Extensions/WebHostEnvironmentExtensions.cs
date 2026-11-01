/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Diagnostics.CodeAnalysis;
using Umbraco.Extensions;

namespace Microsoft.AspNetCore.Hosting
{
    internal static partial class WebHostEnvironmentExtensions
    {
        // Maps the path against the web root, but only if the result is inside the web root.
        // Rejects traversal (e.g. `../`) and absolute paths (e.g. `C:\`, `/etc/`) that resolve outside of it.
        public static bool TryMapPathWebRoot(this IWebHostEnvironment webHostEnvironment, string path, [NotNullWhen(true)] out string? mappedPath)
        {
            mappedPath = null;

            if (string.IsNullOrWhiteSpace(webHostEnvironment.WebRootPath) == true)
            {
                return false;
            }

            string root, full;

            try
            {
                root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(webHostEnvironment.WebRootPath));
                full = Path.TrimEndingDirectorySeparator(Path.GetFullPath(webHostEnvironment.MapPathWebRoot(path)));
            }
            catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
            {
                return false;
            }

            var comparison = OperatingSystem.IsWindows() == true
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            if (string.Equals(full, root, comparison) == true || full.StartsWith(root + Path.DirectorySeparatorChar, comparison) == true)
            {
                mappedPath = full;
                return true;
            }

            return false;
        }

        public static bool WebPathExists(this IWebHostEnvironment webHostEnvironment, string path)
        {
            var webPath = webHostEnvironment.MapPathWebRoot(path);
            var fileInfo = webHostEnvironment.WebRootFileProvider.GetFileInfo(webPath);
            return fileInfo.Exists;
        }
    }
}
