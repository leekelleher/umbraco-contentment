/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class StackDisplayMode : IContentBlocksDisplayMode
    {
        public string Name => "Stack";

        public string Description => "Blocks will be displayed in a single column stack.";

        public string Icon => ContentBlocksDataEditor.DataEditorIcon;

        public string View => IOHelper.ResolveUrl(Constants.Internals.EditorsPathRoot + "content-blocks.html");

        public Dictionary<string, object> DefaultValues => default;

        public Dictionary<string, object> DefaultConfig => new Dictionary<string, object>
        {
            { "sortableAxis", 'y' },
            { "enablePreview", Constants.Values.True },
            { "allowCopy", Constants.Values.True },
            { "allowCreateContentTemplate", Constants.Values.True },
        };

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "allowCopy",
                Name = "Allow copy?",
                Description = "Select to enable copying content blocks.",
                View = "views/propertyeditors/boolean/boolean.html",
                Config = new Dictionary<string, object>
                {
                    { "default", Constants.Values.True },
                }
            },
            new ConfigurationField
            {
                Key = "allowCreateContentTemplate",
                Name = "Allow create content template?",
                Description = "Select to enable the 'Create content template' feature.",
                View = "views/propertyeditors/boolean/boolean.html",
                Config = new Dictionary<string, object>
                {
                    { "default", Constants.Values.True },
                }
            }
        };

        public OverlaySize OverlaySize => OverlaySize.Small;
    }
}
