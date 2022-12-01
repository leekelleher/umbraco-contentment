/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET6_0_OR_GREATER
using Umbraco.Extensions;
#endif

namespace Microsoft.AspNetCore.Hosting
{
    internal static partial class WebHostEnvironmentExtensions
    {
        public static bool WebPathExists(this IWebHostEnvironment webHostEnvironment, string path)
        {
            var webPath = webHostEnvironment.MapPathWebRoot(path);
#if NET472
            return System.IO.File.Exists(webPath);
#else
            var fileInfo = webHostEnvironment.WebRootFileProvider.GetFileInfo(webHostEnvironment.MapPathWebRoot(path));
            return fileInfo.Exists;
#endif
        }
    }
}
