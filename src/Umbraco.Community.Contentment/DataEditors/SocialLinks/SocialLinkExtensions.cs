/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
#else
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.DependencyInjection;
#endif

#if NET472
namespace Umbraco.Web
#else
namespace Umbraco.Extensions
#endif
{
    public static class SocialLinkExtensions
    {
#if NET472
        private static IIconService _iconService => DependencyResolver.Current.GetService<IIconService>();
#else
        private static IIconService _iconService => StaticServiceProvider.Instance.GetRequiredService<IIconService>();
#endif

        public static string GetIconSvgString(this SocialLink link, IIconService iconService = null)
        {
            return string.IsNullOrWhiteSpace(link?.Network) == false
                ? (iconService ?? _iconService)?.GetIcon($"icon-{link.Network}")?.SvgString
                : default;
        }

        public static bool TryGetIconSvgString(this SocialLink link, out string iconSvg, IIconService iconService = null)
        {
            iconSvg = link.GetIconSvgString(iconService);
            return iconSvg != null;
        }
    }
}
