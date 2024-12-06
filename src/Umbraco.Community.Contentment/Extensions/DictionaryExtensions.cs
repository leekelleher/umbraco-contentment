/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Umbraco.Extensions
{
    internal static class DictionaryExtensions
    {
        internal static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key) == false)
            {
                dictionary.Add(key, value);

                return true;
            }

            return false;
        }

        public static TValueOut? GetValueAs<TKey, TValue, TValueOut>(this IDictionary<TKey, TValue> config, TKey key, TValueOut? defaultValue = default)
        {
            if (config.TryGetValue(key, out var tmp) == true)
            {
                if (tmp == null)
                {
                    return defaultValue;
                }

                if (tmp is TValueOut value)
                {
                    return value;
                }

                var attempt = tmp.TryConvertTo<TValueOut>();
                if (attempt.Success == true)
                {
                    return attempt.Result;
                }
            }

            return defaultValue;
        }

        public static bool TryGetValueAs<TKey, TValue, TValueOut>(this IDictionary<TKey, TValue> config, TKey key, out TValueOut? value)
        {
            if (config.TryGetValue(key, out var tmp1) == true)
            {
                if (tmp1 is TValueOut tmp2)
                {
                    value = tmp2;
                    return true;
                }

                var attempt = tmp1.TryConvertTo<TValueOut>();
                if (attempt.Success == true)
                {
                    value = attempt.Result;
                    return attempt.Success;
                }
            }

            value = default;
            return false;
        }
    }
}
