/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class TagsDataListEditor : IDataListEditor
    {
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "tags.html";

        public string Name => "Tags";

        public string Description => "Select items from an Umbraco Tags-like interface.";

        public string Icon => "icon-fa fa-tags";

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ShowIconsConfigurationField(),
            new ConfigurationField
            {
                Key ="confirmRemoval",
                Name = "Confirm removals?",
                Description = "Select to enable a confirmation prompt when removing an item.",
                View = "boolean",
            }
        };

        public Dictionary<string, object> DefaultValues => default;

        public Dictionary<string, object> DefaultConfig => default;

        public bool HasMultipleValues(Dictionary<string, object> config) => true;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public string View => DataEditorViewPath;
    }
}
