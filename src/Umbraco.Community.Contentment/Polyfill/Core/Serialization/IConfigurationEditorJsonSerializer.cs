/* This Source Code has been copied from Lee Kelleher's Umbraco Polyfill library.
 * https://github.com/leekelleher/umbraco-polyfill/blob/main/src/Core/Serialization/IConfigurationEditorJsonSerializer.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Core.Serialization
{
    internal interface IConfigurationEditorJsonSerializer : IJsonSerializer
    { }

    internal sealed class ConfigurationEditorJsonSerializer : UmbracoPolyfillJsonSerializer, IConfigurationEditorJsonSerializer
    {
        public ConfigurationEditorJsonSerializer()
        {
            JsonSerializerSettings.Converters.Add(new FuzzyBooleanConverter());
            JsonSerializerSettings.ContractResolver = new ConfigurationCustomContractResolver();
        }

        private class ConfigurationCustomContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                // base.CreateProperty deals with [JsonProperty("name")]
                var property = base.CreateProperty(member, memberSerialization);

                // override with our custom attribute, if any
                var attribute = member.GetCustomAttribute<ConfigurationFieldAttribute>();
                if (attribute != null) property.PropertyName = attribute.Key;

                // for value types,
                //  don't try to deserialize nulls (in legacy json)
                //  no impact on serialization (value cannot be null)
                if (member is PropertyInfo propertyInfo && propertyInfo.PropertyType.IsValueType)
                    property.NullValueHandling = NullValueHandling.Ignore;

                return property;
            }
        }
    }
}
#endif
