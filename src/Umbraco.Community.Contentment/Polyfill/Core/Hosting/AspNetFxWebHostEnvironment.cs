/* This Source Code has been copied from Lee Kelleher's Umbraco Polyfill library.
 * https://github.com/leekelleher/umbraco-polyfill/blob/main/src/Core/Hosting/AspNetFxWebHostEnvironment.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Microsoft.AspNetCore.Hosting;
using Umbraco.Core.IO;

namespace Umbraco.Core.Hosting
{
    internal sealed class AspNetFxWebHostEnvironment : IWebHostEnvironment
    {
        public string MapPathWebRoot(string path)
        {
            return IOHelper.MapPath(path);
        }
    }
}
#endif
