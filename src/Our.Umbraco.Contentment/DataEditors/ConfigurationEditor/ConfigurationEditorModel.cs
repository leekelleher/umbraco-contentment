/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Newtonsoft.Json;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class ConfigurationEditorModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("fields")]
        public IEnumerable<ConfigurationField> Fields { get; set; }

        [JsonProperty("defaultValues", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> DefaultValues { get; set; }

        [JsonProperty("nameTemplate", NullValueHandling = NullValueHandling.Ignore)]
        public string NameTemplate { get; set; }
    }
}
