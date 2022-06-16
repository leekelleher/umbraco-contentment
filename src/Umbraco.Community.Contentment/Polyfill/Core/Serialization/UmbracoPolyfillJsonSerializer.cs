/* This Source Code has been copied from Lee Kelleher's Umbraco Polyfill library.
 * https://github.com/leekelleher/umbraco-polyfill/blob/main/src/Core/Serialization/UmbracoPolyfillJsonSerializer.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Umbraco.Core.Serialization
{
    internal class UmbracoPolyfillJsonSerializer : IJsonSerializer
    {
        protected static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter>()
            {
                new StringEnumConverter()
            }
        };

        public T Deserialize<T>(string input) => JsonConvert.DeserializeObject<T>(input, JsonSerializerSettings);

        public T DeserializeSubset<T>(string input, string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var root = JsonConvert.DeserializeObject<JObject>(input);

            var jToken = root.SelectToken(key);

            switch (jToken)
            {
                case JArray jArray:
                    return jArray.ToObject<T>();

                case JObject jObject:
                    return jObject.ToObject<T>();

                default:
                    return jToken is null ? default : jToken.Value<T>();
            }
        }

        public string Serialize(object input) => JsonConvert.SerializeObject(input, JsonSerializerSettings);
    }
}
#endif
