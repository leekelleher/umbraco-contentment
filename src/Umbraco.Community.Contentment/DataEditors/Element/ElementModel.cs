/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
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
    internal class ElementModel
    {
        public Guid ElementType { get; set; }

        public Guid Key { get; set; }

        // TODO: LK:2019-09-02] Need to look for a `string` -> `Udi` JSON converter.
        // https://github.com/umbraco/Umbraco-CMS/blob/853087a75044b814df458457dc9a1f778cc89749/src/Umbraco.Core/Serialization/UdiJsonConverter.cs
        // https://github.com/umbraco/Umbraco-CMS/blob/853087a75044b814df458457dc9a1f778cc89749/src/Umbraco.Core/Serialization/KnownTypeUdiJsonConverter.cs
        // https://github.com/umbraco/Umbraco-CMS/blob/853087a75044b814df458457dc9a1f778cc89749/src/Umbraco.Core/Udi.cs
        //[JsonConverter(typeof(UdiJsonConverter))]
        public string Udi { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public Dictionary<string, object> Value { get; set; }
    }
}
