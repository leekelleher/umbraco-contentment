/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ContentBlocksTypesConfigurationField : ConfigurationField
    {
        internal const string ContentBlockTypes = "contentBlockTypes";

        public ContentBlocksTypesConfigurationField(
            IContentTypeService contentTypeService,
            ConfigurationEditorService configurationEditorService)
        {
            var items = contentTypeService
                .GetAllElementTypes()
                .OrderBy(x => x.Name)
                .Select(x => new ConfigurationEditorModel
                {
                    Type = x.Key.ToString(),
                    Name = x.Name,
                    Description = x.Alias,
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
                        { "nameTemplate", $"{x.Name} {{{{ $index + 1 }}}}" },
                    },
                    Fields = configurationEditorService.GetConfigurationFields(typeof(ContentBlocksTypeConfiguration))
                });

            Key = ContentBlockTypes;
            Name = "Block types";
            Description = "Configure the block types to use.";
            View = IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorViewPath);
            Config = new Dictionary<string, object>
            {
                { AllowDuplicatesConfigurationField.AllowDuplicates, Constants.Values.False },
                { EnableFilterConfigurationField.EnableFilter, Constants.Values.True },
                { ConfigurationEditorConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) },
                { ConfigurationEditorConfigurationEditor.Items, items },
                { OverlaySizeConfigurationField.OverlaySize, OverlaySizeConfigurationField.Small },
            };
        }
    }
}
