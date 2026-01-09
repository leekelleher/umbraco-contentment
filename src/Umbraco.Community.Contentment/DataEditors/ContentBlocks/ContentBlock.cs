/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Text.Json.Serialization;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class ContentBlock
    {
        public ContentBlock()
        {
            Value = new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase);
        }

        [JsonPropertyName("elementType")]
        public Guid ElementType { get; set; }

        [JsonPropertyName("key")]
        public Guid Key { get; set; }

        [JsonPropertyName("value")]
        public Dictionary<string, object?> Value { get; set; }
    }
}
