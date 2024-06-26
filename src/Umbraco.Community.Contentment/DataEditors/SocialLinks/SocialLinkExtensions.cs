/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Extensions
{
    public static class SocialLinkExtensions
    {
        private static IIconService _iconService => StaticServiceProvider.Instance.GetRequiredService<IIconService>();

        public static string? GetIconSvgString(this SocialLink link, IIconService? iconService = default)
        {
            return string.IsNullOrWhiteSpace(link?.Network) == false
                ? (iconService ?? _iconService)?.GetIcon($"icon-{link.Network}")?.SvgString
                : default;
        }

        public static bool TryGetIconSvgString(this SocialLink link, out string? iconSvg, IIconService? iconService = default)
        {
            iconSvg = link.GetIconSvgString(iconService);
            return iconSvg is not null;
        }
    }
}
