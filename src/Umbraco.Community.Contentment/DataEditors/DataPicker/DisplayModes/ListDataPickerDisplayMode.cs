/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class ListDataPickerDisplayMode : IDataPickerDisplayMode
    {
        public string Name => "List";

        public string Description => "Items will be displayed in a list similar to a content picker.";

        public string Icon => DataListDataEditor.DataEditorIcon;

        public string? Group => default;

        public string View => Constants.Internals.EditorsPathRoot + "data-picker.html";

        public Dictionary<string, object>? DefaultValues => default;

        public Dictionary<string, object> DefaultConfig => new()
        {
            { "displayMode", "list" },
        };

        public IEnumerable<ConfigurationField> Fields => Enumerable.Empty<ConfigurationField>();

        public OverlaySize OverlaySize => OverlaySize.Small;
    }
}
