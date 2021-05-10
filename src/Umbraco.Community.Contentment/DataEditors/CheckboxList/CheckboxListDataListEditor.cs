/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class CheckboxListDataListEditor : IDataListEditor
    {
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "checkbox-list.html";

        public string Name => "Checkbox List";

        public string Description => "Select multiple values from a list of checkboxes.";

        public string Icon => "icon-fa fa-check-square-o";

        public string Group => default;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ConfigurationField
            {
                Key = "checkAll",
                Name = "Check all?",
                Description = "Include a toggle button to select or deselect all the options?",
                View = "boolean",
            },
            new ShowDescriptionsConfigurationField(),
            new ShowIconsConfigurationField(),
        };

        public Dictionary<string, object> DefaultValues => default;

        public Dictionary<string, object> DefaultConfig => default;

        public bool HasMultipleValues(Dictionary<string, object> config) => true;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public string View => DataEditorViewPath;
    }
}
