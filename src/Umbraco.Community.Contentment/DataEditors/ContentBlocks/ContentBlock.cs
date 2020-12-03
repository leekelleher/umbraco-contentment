/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Umbraco.Community.Contentment.DataEditors
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    internal sealed class ContentBlock
    {
        public Guid ElementType { get; set; }

        public Guid Key { get; set; }

        public string Icon { get; set; }

        public Dictionary<string, object> Value { get; set; }
    }
}
