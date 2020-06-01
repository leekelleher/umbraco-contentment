/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ContentBlocksTypesConfigurationField : ConfigurationField
    {
        internal const string ContentBlockTypes = "contentBlockTypes";

        public ContentBlocksTypesConfigurationField(IEnumerable<IContentType> elementTypes)
        {
            var fields = new[]
            {
                new ConfigurationField
                {
                    Key = "elementType",
                    Name = "Element type",
                    View = IOHelper.ResolveUrl(Constants.Internals.EditorsPathRoot + "readonly-node-preview.html"),
                    HideLabel = true,
                },
                new ConfigurationField
                {
                    Key = "nameTemplate",
                    Name = "Name template",
                    View = "textstring",
                    Description = "Enter an AngularJS expression to evaluate against each block for its name."
                },
                new ConfigurationField
                {
                    Key = "overlaySize",
                    Name = "Editor overlay size",
                    Description = "Select the size of the overlay editing panel. By default this is set to 'large'. However if the editor fields require a smaller panel, select 'small'.",
                    View = IOHelper.ResolveUrl(RadioButtonListDataListEditor.DataEditorViewPath),
                    Config = new Dictionary<string, object>
                    {
                        { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                            {
                                new DataListItem { Name = "Small", Value = "small" },
                                new DataListItem { Name = "Medium", Value = "medium" },
                                new DataListItem { Name = "Large", Value = "large" }
                            }
                        },
                        { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, "small" }
                    }
                }
            };

            var items = elementTypes
                .OrderBy(x => x.Name)
                .Select(x => new ConfigurationEditorModel
                {
                    Key = x.Key.ToString(),
                    Name = x.Name,
                    Description = string.IsNullOrWhiteSpace(x.Description) == false ? x.Description : x.Alias,
                    Icon = x.Icon,
                    DefaultValues = new Dictionary<string, object>
                    {
                        { "elementType", new DataListItem
                            {
                                Name = x.Name,
                                Description = x.GetUdi().ToString(),
                                Icon = x.Icon
                            }
                        },
                        { "nameTemplate", $"{x.Name} {{{{ $index }}}}" },
                    },
                    Fields = fields,
                    OverlaySize = OverlaySize.Small
                });

            Key = ContentBlockTypes;
            Name = "Block types";
            Description = "Configure the block types to use.";
            View = IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorViewPath);
            Config = new Dictionary<string, object>
            {
                { "allowDuplicates", Constants.Values.False },
                { EnableFilterConfigurationField.EnableFilter, Constants.Values.True },
                { Constants.Conventions.ConfigurationFieldAliases.OverlayView, IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) },
                { Constants.Conventions.ConfigurationFieldAliases.Items, items },
                { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
            };
        }
    }
}
