/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472

using System.ComponentModel;
using Microsoft.AspNetCore.Hosting;
using Umbraco.Core.Hosting;

namespace Microsoft.AspNetCore.Hosting
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IWebHostEnvironment
    {
        string MapPathWebRoot(string path);
    }
}

namespace Umbraco.Community.Contentment.Polyfill
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class ContenmentWebHostEnvironment : IWebHostEnvironment
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ContenmentWebHostEnvironment(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public string MapPathWebRoot(string path)
        {
            return _hostingEnvironment.MapPathWebRoot(path);
        }
    }
}

#elif NET5_0

using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Web.Common.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    internal static class WebHostEnvironmentExtensions
    {
        public static string MapPathWebRoot(this IWebHostEnvironment webHostEnvironment, string path)
        {
            var hostingEnvironment = StaticServiceProvider.Instance.GetRequiredService<Umbraco.Cms.Core.Hosting.IHostingEnvironment>();
            return hostingEnvironment.MapPathWebRoot(path);
        }
    }
}

#endif
