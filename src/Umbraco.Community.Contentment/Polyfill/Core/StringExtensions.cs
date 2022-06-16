/* This Source Code has been copied from Lee Kelleher's Umbraco Polyfill library.
 * https://github.com/leekelleher/umbraco-polyfill/blob/main/src/Core/StringExtensions.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Core.Strings;

namespace Umbraco.Core
{
    internal static class StringExtensions
    {
        public static string ToSafeAlias(this string alias, IShortStringHelper shortStringHelper) => alias.ToSafeAlias();

        public static string ToSafeAlias(this string alias, IShortStringHelper shortStringHelper, bool camel) => alias.ToSafeAlias(camel);

        public static string ToSafeAlias(this string alias, IShortStringHelper shortStringHelper, string culture) => alias.ToSafeAlias(culture);

        public static string ToUrlSegment(this string text, IShortStringHelper shortStringHelper, string culture) => text.ToUrlSegment(culture);

        public static string ToCleanString(this string text, IShortStringHelper shortStringHelper, CleanStringType stringType) => text.ToCleanString(stringType);

        public static string ToCleanString(this string text, IShortStringHelper shortStringHelper, CleanStringType stringType, char separator) => text.ToCleanString(stringType, separator);

        public static string ToCleanString(this string text, IShortStringHelper shortStringHelper, CleanStringType stringType, string culture) => text.ToCleanString(stringType, culture);

        public static string SplitPascalCasing(this string phrase, IShortStringHelper shortStringHelper) => phrase.SplitPascalCasing();

        public static string ToSafeFileName(this string text, IShortStringHelper shortStringHelper) => text.ToSafeFileName();

        public static string ToSafeFileName(this string text, IShortStringHelper shortStringHelper, string culture) => text.ToSafeFileName(culture);
    }
}
#endif
