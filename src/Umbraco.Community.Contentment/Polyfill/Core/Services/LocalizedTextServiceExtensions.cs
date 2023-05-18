/* Copyright © 2013-present Umbraco.
 * This Source Code has been derived from Umbraco CMS.
 * https://github.com/umbraco/Umbraco-CMS/blob/release-8.18.8/src/Umbraco.Core/Services/LocalizedTextServiceExtensions.cs#L195-L212
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Core.Dictionary;

namespace Umbraco.Core.Services
{
    internal static partial class LocalizedTextServiceExtensions
    {
        public static string UmbracoDictionaryTranslate(
            this ILocalizedTextService manager,
            ICultureDictionary cultureDictionary,
            string text)
        {
            if (text == null)
            {
                return null;
            }

            if (text.StartsWith("#") == false)
            {
                return text;
            }

            text = text.Substring(1);

            var value = cultureDictionary[text];

            if (string.IsNullOrWhiteSpace(value) == false)
            {
                return value;
            }

#pragma warning disable CS0618 // Type or member is obsolete
            value = manager.Localize(text.Replace('_', '/'));
#pragma warning restore CS0618 // Type or member is obsolete

            return value.StartsWith("[") == true
                ? text
                : value;
        }
    }
}
#endif
