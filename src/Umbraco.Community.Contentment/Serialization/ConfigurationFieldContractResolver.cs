/* Copyright © 2013-present Umbraco.
 * This Source Code has been derived from Umbraco CMS.
 * https://github.com/umbraco/Umbraco-CMS/blob/release-8.0.2/src/Umbraco.Core/PropertyEditors/ConfigurationEditor.cs#L127-L145
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.Serialization
{
    internal sealed class ConfigurationFieldContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            var attribute = member.GetCustomAttribute<ConfigurationFieldAttribute>();
            if (attribute != null)
            {
                if (attribute.Type != null)
                {
                    // TODO: [LK:2019-11-15] Submit this patch to Umbraco core
                    // https://github.com/umbraco/Umbraco-CMS/blob/release-8.0.2/src/Umbraco.Core/PropertyEditors/ConfigurationEditor.cs#L136
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
