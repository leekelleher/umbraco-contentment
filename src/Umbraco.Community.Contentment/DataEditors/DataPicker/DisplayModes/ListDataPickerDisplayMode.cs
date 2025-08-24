/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class ListDataPickerDisplayMode : IContentmentDisplayMode
    {
        public string Name => "List";

        public string Description => "Items will be displayed in a list similar to a content picker.";

        public string Icon => DataListDataEditor.DataEditorIcon;

        public string? Group => default;

        [Obsolete("To be removed in Contentment 8.0.")]
        public string View => string.Empty;

        public string PropertyEditorUiAlias => "Umb.Contentment.DisplayMode.List";

        public Dictionary<string, object>? DefaultValues => default;

        public Dictionary<string, object>? DefaultConfig => default;

        public IEnumerable<ContentmentConfigurationField> Fields => Enumerable.Empty<ContentmentConfigurationField>();

        public OverlaySize OverlaySize => OverlaySize.Small;
    }
}
