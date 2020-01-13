/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public sealed class ConfigurationEditorModel
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public IEnumerable<ConfigurationField> Fields { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> DefaultValues { get; set; }
    }
}
