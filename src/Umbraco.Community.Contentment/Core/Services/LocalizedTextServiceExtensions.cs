/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472 == false
using Umbraco.Cms.Core.Services;
#endif
using HappyConstants = Umbraco.Community.Contentment.Constants;

#if NET472
namespace Umbraco.Core.Services
#else
namespace Umbraco.Extensions
#endif
{
    internal static partial class LocalizedTextServiceExtensions
    {
        public static string LocalizeContentment(this ILocalizedTextService service, string key, string fallback = null)
        {
            var localized = service.Localize(HappyConstants.Internals.ProjectAlias, key);

            return string.IsNullOrWhiteSpace(localized) == false && localized.InvariantEquals($"[{key}]") == false
                ? localized
                : fallback ?? localized;
        }
    }
}
