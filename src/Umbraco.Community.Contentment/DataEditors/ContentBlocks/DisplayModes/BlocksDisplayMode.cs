/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;
using UmbIcons = Umbraco.Cms.Core.Constants.Icons;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class BlocksDisplayMode : IContentBlocksDisplayMode
    {
        public string Name => "Blocks";

        public string Description => "Blocks will be displayed in a list similar to the Block List editor.";

        public string Icon => UmbIcons.ListView;

        public string? Group => default;

        public string View => Constants.Internals.EditorsPathRoot + "content-blocks.html";

        public Dictionary<string, object> DefaultValues => new()
        {
            { "allowCopy", Constants.Values.True },
            { "allowCreateContentTemplate", Constants.Values.True },
        };

        public Dictionary<string, object> DefaultConfig => new()
        {
            { "allowCopy", Constants.Values.True },
            { "allowCreateContentTemplate", Constants.Values.True },
            { ContentBlocksConfigurationEditor.DisplayMode, "blocks" },
            { "enablePreview", Constants.Values.True },
        };

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ContentmentConfigurationField
            {
                Key = "allowCopy",
                Name = "Allow copy?",
                Description = "Select to enable copying content blocks.",
                View = "views/propertyeditors/boolean/boolean.html",
            },
            new ContentmentConfigurationField
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
