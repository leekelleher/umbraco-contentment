﻿/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Umbraco.Community.Contentment.DataEditors
{
    [JsonObject(
        ItemNullValueHandling = NullValueHandling.Ignore,
        NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public sealed class DataListItem
    {
        public string Description { get; set; }

        public bool Disabled { get; set; }

        public string Icon { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
