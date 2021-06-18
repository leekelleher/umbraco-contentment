/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;
using UmbIcons = Umbraco.Core.Constants.Icons;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class BlocksDisplayMode : IContentBlocksDisplayMode
    {
        public string Name => "Blocks";

        public string Description => "Blocks will be displayed in a list similar to the Block List editor.";

        public string Icon => UmbIcons.ListView;

        public string Group => default;

        public string View => Constants.Internals.EditorsPathRoot + "content-blocks.html";

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "allowCopy", Constants.Values.True },
            { "allowCreateContentTemplate", Constants.Values.True },
        };

        public Dictionary<string, object> DefaultConfig => new Dictionary<string, object>
        {
            { "enablePreview", Constants.Values.False },
            { "allowCopy", Constants.Values.True },
            { "allowCreateContentTemplate", Constants.Values.True },
        };

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new NotesConfigurationField($@"<details class=""well well-small"" open>
<summary><strong>A note about block type previews.</strong></summary>
<p>Currently, the preview feature for block types has not been implemented for the {Name} display mode and has been temporarily disabled.</p>
</details>", true),
            new ConfigurationField
            {
                Key = "allowCopy",
                Name = "Allow copy?",
                Description = "Select to enable copying content blocks.",
                View = "views/propertyeditors/boolean/boolean.html",
            },
            new ConfigurationField
            {
                Key = "allowCreateContentTemplate",
                Name = "Allow create content template?",
                Description = "Select to enable the 'Create content template' feature.",
                View = "views/propertyeditors/boolean/boolean.html",
            }
        };

        public OverlaySize OverlaySize => OverlaySize.Small;
    }
}
