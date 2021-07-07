/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ContentBlocksTypesConfigurationField : ConfigurationField
    {
        internal const string ContentBlockTypes = "contentBlockTypes";
        private readonly IIOHelper _ioHelper;

        public ContentBlocksTypesConfigurationField(IEnumerable<IContentType> elementTypes, IIOHelper ioHelper)
        {
            _ioHelper = ioHelper;

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
                        { "nameTemplate", $"{x.Name} {{{{ $index }}}}" },
                    },
                    Fields = GetConfigurationFields(x),
                    OverlaySize = OverlaySize.Small
                });

            Key = ContentBlockTypes;
            Name = "Block types";
            Description = "Configure the block types to use.";
            View = ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorViewPath);
            Config = new Dictionary<string, object>
            {
                { Constants.Conventions.ConfigurationFieldAliases.AddButtonLabelKey, "contentment_configureElementType" },
                { "allowDuplicates", Constants.Values.False },
                { EnableFilterConfigurationField.EnableFilter, Constants.Values.True },
                { Constants.Conventions.ConfigurationFieldAliases.OverlayView, ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) },
                { Constants.Conventions.ConfigurationFieldAliases.Items, items },
                { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
            };
        }

        private IEnumerable<ConfigurationField> GetConfigurationFields(IContentType contentType)
        {
            return new[]
            {
                new ConfigurationField
                {
                    Key = "elementType",
                    Name = "Element type",
                    View = _ioHelper.ResolveRelativeOrVirtualUrl(Constants.Internals.EditorsPathRoot + "readonly-node-preview.html"),
                    Config = new Dictionary<string,object>
                    {
                        { "name", contentType.Name },
                        { "icon", contentType.Icon },
                        { "description", contentType.GetUdi().ToString() },
                    },
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
                    Description = "Select the size of the overlay editing panel. By default this is set to 'small'. However if the editor fields require a wider panel, please select 'medium' or 'large'.",
                    View = _ioHelper.ResolveRelativeOrVirtualUrl(RadioButtonListDataListEditor.DataEditorViewPath),
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
                },
                new ConfigurationField
                {
                    Key = "enablePreview",
                    Name = "Enable preview?",
                    Description = "Select to enable a rich preview for this content block type.",
                    View = "views/propertyeditors/boolean/boolean.html",
                    Config = new Dictionary<string, object>
                    {
                        { "default", Constants.Values.False }
                    }
                }
            };
        }
    }
}
