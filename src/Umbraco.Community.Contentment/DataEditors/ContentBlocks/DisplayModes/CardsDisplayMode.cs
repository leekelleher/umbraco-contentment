///* Copyright © 2021 Lee Kelleher.
// * This Source Code Form is subject to the terms of the Mozilla Public
// * License, v. 2.0. If a copy of the MPL was not distributed with this
// * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

//using Umbraco.Cms.Core.PropertyEditors;

//namespace Umbraco.Community.Contentment.DataEditors
//{
//    internal class CardsDisplayMode : IContentBlocksDisplayMode
//    {
//        public string Name => "Cards";

//        public string Description => "Blocks will be displayed as cards.";

//        public string Icon => "icon-playing-cards";

//        public string? Group => default;

//        [Obsolete("To be removed in Contentment 8.0.")]
//        public string View => string.Empty;

//        public string PropertyEditorUiAlias => "Umb.Contentment.DisplayMode.Cards";

//        public Dictionary<string, object> DefaultValues => new()
//        {
//            { "allowCopy", true },
//            { "allowCreateContentTemplate", false },
//        };

//        public Dictionary<string, object> DefaultConfig => new()
//        {
//            { "allowCopy", true },
//            { "allowCreateContentTemplate", false },
//            { "displayMode", "cards" },
//            { "enablePreview", false },
//        };

//        public IEnumerable<ContentmentConfigurationField> Fields => new ContentmentConfigurationField[]
//        {
//            new NotesConfigurationField($@"<details class=""well well-small"" open>
//<summary><strong>A note about block type previews.</strong></summary>
//<p>Currently, the preview feature for block types has not been implemented for the {Name} display mode and has been temporarily disabled.</p>
//</details>", true),
//            new ContentmentConfigurationField
//            {
//                Key = "allowCopy",
//                Name = "Allow copy?",
//                Description = "Select to enable copying content blocks.",
//                PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
//            },
//            new ContentmentConfigurationField
//            {
//                Key = "allowCreateContentTemplate",
//                Name = "Allow create content template?",
//                Description = "Select to enable the 'Create content template' feature.",
//                PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
//            }
//        };

//        public OverlaySize OverlaySize => OverlaySize.Small;
//    }
//}
