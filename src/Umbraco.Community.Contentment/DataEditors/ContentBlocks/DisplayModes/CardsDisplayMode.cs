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

        public string View => Constants.Internals.EditorsPathRoot + "content-cards.html";

        public Dictionary<string, object> DefaultValues => default;

        public Dictionary<string, object> DefaultConfig => new Dictionary<string, object>
        {
            { "enablePreview", Constants.Values.False },
            { "allowCopy", Constants.Values.False },
            { "allowCreateContentTemplate", Constants.Values.False },
        };

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new NotesConfigurationField(_ioHelper, $@"<details class=""well well-small"" open>
<summary><strong>A note about block type previews.</strong></summary>
<p>Currently, the preview feature for block types has not been implemented for the {Name} display mode and has been temporarily disabled.</p>
</details>", true),
        };

        public OverlaySize OverlaySize => OverlaySize.Small;
    }
}
