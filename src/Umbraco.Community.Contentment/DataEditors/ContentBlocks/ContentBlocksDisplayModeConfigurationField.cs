/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class ContentBlocksDisplayModeConfigurationField : ConfigurationField
    {
        public const string DisplayMode = "displayMode";
        public const string Blocks = ContentBlocksDataEditor.DataEditorBlocksViewPath;
        public const string List = ContentBlocksDataEditor.DataEditorListViewPath;

        public ContentBlocksDisplayModeConfigurationField(string defaultValue = Blocks)
            : base()
        {
            Key = DisplayMode;
            Name = "Display mode?";
            Description = "Select to display the elements in a list or as stacked blocks.";
            View = IOHelper.ResolveUrl(RadioButtonListDataListEditor.DataEditorViewPath);
            Config = new Dictionary<string, object>
            {
                { Constants.Conventions.ConfigurationFieldAliases.Items, new DataListItem[]
                    {
                        new DataListItem { Name = nameof(Blocks), Value = Blocks, Description = "This will display as stacked blocks." },
                        new DataListItem { Name = nameof(List), Value = List, Description = "This will display similar to a content picker." },
                    }
                },
                { ShowDescriptionsConfigurationField.ShowDescriptions, Constants.Values.True },
                { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, defaultValue }
            };
        }
    }
}
