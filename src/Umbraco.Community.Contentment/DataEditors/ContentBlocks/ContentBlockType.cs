/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Text.Json.Serialization;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ContentBlockType
    {
        public string? Alias { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Icon { get; set; }

        public string? Name { get; set; }

        public Guid Key { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? NameTemplate { get; set; }

        public string? OverlaySize { get; set; }

        public bool PreviewEnabled { get; set; }

        public IEnumerable<BlueprintItem> Blueprints { get; set; } = Enumerable.Empty<BlueprintItem>();

        internal sealed class BlueprintItem
        {
            public string? Name { get; set; }

            public int Id { get; set; }
        }
    }
}
