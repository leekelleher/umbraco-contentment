/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.ComponentModel;
using System.Text.Json.Serialization;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ConfigurationEditorModel : IConfigurationEditorItem
    {
        public required string Key { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public string? Icon { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Group { get; set; }

        public IEnumerable<ContentmentConfigurationField> Fields { get; set; } = [];

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? DefaultValues { get; set; }

        public OverlaySize OverlaySize { get; set; } = OverlaySize.Small;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string>? Expressions { get; set; }
    }
}
