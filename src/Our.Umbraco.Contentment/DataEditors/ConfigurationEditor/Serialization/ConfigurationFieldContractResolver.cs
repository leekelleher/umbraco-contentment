/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    // https://github.com/umbraco/Umbraco-CMS/blob/release-8.0.2/src/Umbraco.Core/PropertyEditors/ConfigurationEditor.cs#L127-L145
    internal class ConfigurationFieldContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            var attribute = member.GetCustomAttribute<ConfigurationFieldAttribute>();
            if (attribute != null)
            {
                if (attribute.Type != null)
                {
                    var field = (ConfigurationField)Activator.CreateInstance(attribute.Type);
                    property.PropertyName = field.Key;
                }
                else
                {
                    property.PropertyName = attribute.Key;
                }
            }

            if (member is PropertyInfo propertyInfo && propertyInfo.PropertyType.IsValueType)
            {
                property.NullValueHandling = NullValueHandling.Ignore;
            }

            return property;
        }
    }
}
