/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Umbraco.Cms.Core.Models.PublishedContent
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class SocialLink
    {
        public string? Network { get; set; }

        public string? Name { get; set; }

        public string? Url { get; set; }
    }
}
