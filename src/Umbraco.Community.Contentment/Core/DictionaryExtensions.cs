/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;

namespace Umbraco.Core
{
    internal static class DictionaryExtensions
    {
        public static TValOut GetValueAs<TKey, TVal, TValOut>(this IDictionary<TKey, TVal> config, TKey key, TValOut defaultValue = default)
        {
            if (config.TryGetValue(key, out var tmp))
            {
                if (tmp is TValOut value)
                {
                    return value;
                }

                var attempt = tmp.TryConvertTo<TValOut>();
                if (attempt.Success)
                {
                    return attempt.Result;
                }
            }

            return defaultValue;
        }

        public static bool TryGetValueAs<TKey, TVal, TValOut>(this IDictionary<TKey, TVal> config, TKey key, out TValOut value)
        {
            if (config.TryGetValue(key, out var tmp1))
            {
                if (tmp1 is TValOut tmp2)
                {
                    value = tmp2;
                    return true;
                }

                var attempt = tmp1.TryConvertTo<TValOut>();
                if (attempt.Success)
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
