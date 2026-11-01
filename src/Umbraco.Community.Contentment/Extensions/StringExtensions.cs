// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

namespace Umbraco.Community.Contentment;

internal static class StringExtensions
{
    // Only absolute `http` and `https` URLs are treated as remote, anything else is treated as a local path.
    public static bool IsHttpUrl(this string url)
        => Uri.TryCreate(url, UriKind.Absolute, out var uri) == true
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
