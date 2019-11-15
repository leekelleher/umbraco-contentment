/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Newtonsoft.Json;
using Umbraco.Core.Serialization;

namespace Umbraco.Community.Contentment.Serialization
{
    // TODO: [LK:2019-11-15] If I can remove my custom `ConfigurationFieldContractResolver`,
    // then we can use `Core.PropertyEditors.ConfigurationEditor.ConfigurationJsonSettings` directly.
    internal class ConfigurationFieldJsonSerializerSettings : JsonSerializerSettings
    {
        public ConfigurationFieldJsonSerializerSettings()
            : base()
        {
            ContractResolver = new ConfigurationFieldContractResolver();

            Converters = new List<JsonConverter>(new[]
            {
                new FuzzyBooleanConverter(),
            });
        }
    }
}
