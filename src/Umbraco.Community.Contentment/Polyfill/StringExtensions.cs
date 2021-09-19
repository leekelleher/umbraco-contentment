/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using System.ComponentModel;
using Umbraco.Core.Strings;

namespace Umbraco.Core
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class StringExtensions
    {
        public static string SplitPascalCasing(this string phrase, IShortStringHelper shortStringHelper)
        {
            // https://github.com/umbraco/Umbraco-CMS/blob/release-8.17.0/src/Umbraco.Core/StringExtensions.cs#L1191
            return shortStringHelper.SplitPascalCasing(phrase, ' ');
        }
    }
}
#endif
