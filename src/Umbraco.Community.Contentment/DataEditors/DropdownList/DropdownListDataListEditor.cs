/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class DropdownListDataListEditor : IDataListEditor
    {
        internal const string AllowEmpty = "allowEmpty";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "dropdown-list.html";
        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "DropdownList";

        public string Name => "Dropdown List";

        public string Description => "Select a single value from a dropdown select list.";

        public string Icon => "icon-fa-square-caret-down";

        public string? Group => default;

        public IEnumerable<ContentmentConfigurationField> Fields => new ContentmentConfigurationField[]
        {
            new ContentmentConfigurationField
            {
                Key = AllowEmpty,
                Name = "Allow empty?",
                Description = "Enable to allow an empty option at the top of the dropdown list.<br>When disabled, the default value will be set to the first option.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
                Config = new Dictionary<string, object>
                {
                    { "default", true }
                }
            },
            new ShowDescriptionsConfigurationField(),
            new ShowIconsConfigurationField(),
            //new HtmlAttributesConfigurationField(_ioHelper),
        };

        public Dictionary<string, object> DefaultValues => new()
        {
            { AllowEmpty, true }
        };

        public Dictionary<string, object>? DefaultConfig => default;

        public bool HasMultipleValues(Dictionary<string, object>? config) => false;

        public OverlaySize OverlaySize => OverlaySize.Small;

        [Obsolete("To be removed in Contentment 7.0. Migrate to use `PropertyEditorUiAlias`.")]
        public string View => DataEditorViewPath;

        public string PropertyEditorUiAlias => DataEditorUiAlias;
    }
}
