/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Extensions;

namespace Microsoft.AspNetCore.Hosting
{
    internal static partial class WebHostEnvironmentExtensions
    {
        public static bool WebPathExists(this IWebHostEnvironment webHostEnvironment, string path)
        {
            var webPath = webHostEnvironment.MapPathWebRoot(path);
            var fileInfo = webHostEnvironment.WebRootFileProvider.GetFileInfo(webPath);
            return fileInfo.Exists;
        }
    }
}
