/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoBackofficeSectionsDataSource : DataListToDataPickerSourceBridge, IDataListSource
    {
        public override string Name => "Umbraco Backoffice Sections";

        public override string Description => "Use the backoffice sections to populate the data source.";

        public override string Icon => "icon-block color-red";

        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public override IEnumerable<ContentmentConfigurationField> Fields =>
        [
            new ContentmentConfigurationField
            {
                Key = "unsupported",
                Name = "Unsupported",
                PropertyEditorUiAlias = EditorNotesDataEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { "alertType", "warning" },
                    { "icon", "icon-alert" },
                    { "heading", "Umbraco Backoffice Sections is not yet supported" },
                    { "message", "<p><em>Support will be added in a future version of Contentment.</em></p>" },
                    { "hideLabel", true },
                },
            }
         ];

        public override Dictionary<string, object>? DefaultValues => default;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config) => [];
    }
}
