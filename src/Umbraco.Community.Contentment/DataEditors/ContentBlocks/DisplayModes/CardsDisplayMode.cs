/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
#if NET472
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class CardsDisplayMode : IContentBlocksDisplayMode
    {
        private readonly IIOHelper _ioHelper;

        public CardsDisplayMode(IIOHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }

        public string Name => "Cards";

        public string Description => "Blocks will be displayed as cards.";

        public string Icon => "icon-playing-cards";

        public string Group => default;

        public string View => Constants.Internals.EditorsPathRoot + "content-blocks.html";

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "allowCopy", Constants.Values.True },
            { "allowCreateContentTemplate", Constants.Values.False },
        };

        public Dictionary<string, object> DefaultConfig => new Dictionary<string, object>
        {
            { "allowCopy", Constants.Values.True },
            { "allowCreateContentTemplate", Constants.Values.False },
            { ContentBlocksConfigurationEditor.DisplayMode, "cards" },
            { "enablePreview", Constants.Values.False },
        };

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new NotesConfigurationField(_ioHelper, $@"<details class=""well well-small"" open>
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
