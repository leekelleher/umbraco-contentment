/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class ListDisplayMode : IContentBlocksDisplayMode
    {
        private readonly IIOHelper _ioHelper;

        public ListDisplayMode(IIOHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }

        public string Name => "List";

        public string Description => "Blocks will be displayed in a list similar to a content picker.";

        public string Icon => DataListDataEditor.DataEditorIcon;

        public string Group => default;

        public string View => Constants.Internals.EditorsPathRoot + "content-blocks.html";

        public Dictionary<string, object> DefaultValues => default;

        public Dictionary<string, object> DefaultConfig => new Dictionary<string, object>
        {
            { ContentBlocksConfigurationEditor.DisplayMode, "list" },
            { "enablePreview", Constants.Values.False },
        };

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new NotesConfigurationField(_ioHelper, $@"<details class=""well well-small"" open>
<summary><strong>A note about block type previews.</strong></summary>
<p>Unfortunately, the preview feature for block types is unsupported in the {Name} display mode and will be disabled.</p>
</details>", true),
        };

        public OverlaySize OverlaySize => OverlaySize.Small;
    }
}
