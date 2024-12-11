/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Umbraco.Community.Contentment.DataEditors
{
    [TypeConverter(typeof(DataListItemTypeConverter))]
    public class DataListItem
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Disabled { get; set; } = false;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Group { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Icon { get; set; }

        public string? Name { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object>? Properties { get; set; } = new();

        public string? Value { get; set; }
    }
}
