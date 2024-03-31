/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ContentBlocksTypesConfigurationField : ContentmentConfigurationField
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
            Description = "Configure the element types to be used as blocks.";
            View = ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorViewPath);
            Config = new Dictionary<string, object>
            {
                { Constants.Conventions.ConfigurationFieldAliases.AddButtonLabelKey, "contentment_configureElementType" },
                { EnableFilterConfigurationField.EnableFilter, Constants.Values.True },
                { Constants.Conventions.ConfigurationFieldAliases.OverlayView, ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) ?? string.Empty },
                { Constants.Conventions.ConfigurationFieldAliases.Items, items },
                { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
            };
        }

        private IEnumerable<ConfigurationField> GetConfigurationFields(IContentType contentType)
        {
            return new[]
            {
                new ContentmentConfigurationField
                {
                    Key = "elementType",
                    Name = "Element type",
                    View = _ioHelper.ResolveRelativeOrVirtualUrl(Constants.Internals.EditorsPathRoot + "readonly-node-preview.html"),
                    Config = new Dictionary<string, object>
                    {
                        { "name", contentType.Name ?? contentType.Alias },
                        { "icon", contentType.Icon ?? UmbConstants.Icons.DefaultIcon },
                        { "description", contentType.GetUdi().ToString() },
                    },
                    HideLabel = true,
                },
                new ContentmentConfigurationField
                {
                    Key = "nameTemplate",
                    Name = "Name template",
                    Description = "Enter an AngularJS expression to evaluate against each block for its name.",
                    View = "textstring",
                },
                new ContentmentConfigurationField
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
                new ContentmentConfigurationField
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
