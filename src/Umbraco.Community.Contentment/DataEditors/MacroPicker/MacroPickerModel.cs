/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class MacroPickerModel
    {
        [JsonProperty("udi")]
        public string Udi { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("params")]
        public Dictionary<string, object> Parameters { get; set; }
    }
}
