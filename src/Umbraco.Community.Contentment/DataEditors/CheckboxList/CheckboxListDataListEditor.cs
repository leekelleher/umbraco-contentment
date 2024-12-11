/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class CheckboxListDataListEditor : IDataListEditor
    {
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "checkbox-list.html";
        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "CheckBoxList";

        public string Name => "Checkbox List";

        public string Description => "Select multiple values from a list of checkboxes.";

        public string Icon => "icon-bulleted-list"; // "icon-fa fa-check-square-o";

        public string? Group => default;

        public IEnumerable<ContentmentConfigurationField> Fields => new ContentmentConfigurationField[]
        {
            new ContentmentConfigurationField
            {
                Key = "checkAll",
                Name = "Check all?",
                Description = "Include a toggle button to select or deselect all the options?",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
            },
            new ShowDescriptionsConfigurationField(),
            new ShowIconsConfigurationField(),
        };

        public Dictionary<string, object>? DefaultValues => default;

        public Dictionary<string, object>? DefaultConfig => default;

        public bool HasMultipleValues(Dictionary<string, object>? config) => true;

        public OverlaySize OverlaySize => OverlaySize.Small;

        [Obsolete("To be removed in Contentment 7.0. Migrate to use `PropertyEditorUiAlias`.")]
        public string View => DataEditorViewPath;

        public string PropertyEditorUiAlias => DataEditorUiAlias;
    }
}
