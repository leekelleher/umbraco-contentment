/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET5_0
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Web.Common.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    // NOTE: A polfill for:
    // https://github.com/umbraco/Umbraco-CMS/blob/v10/contrib/src/Umbraco.Web.Common/Extensions/WebHostEnvironmentExtensions.cs
    public static class WebHostEnvironmentExtensions
    {
        public static string MapPathWebRoot(this IWebHostEnvironment webHostEnvironment, string path)
        {
            var hostingEnvironment = StaticServiceProvider.Instance
                .GetRequiredService<Umbraco.Cms.Core.Hosting.IHostingEnvironment>();

            return hostingEnvironment.MapPathWebRoot(path);
        }
    }
}
#endif
