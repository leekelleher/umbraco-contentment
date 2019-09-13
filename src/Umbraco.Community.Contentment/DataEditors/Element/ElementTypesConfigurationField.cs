/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
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
    internal class ElementTypesConfigurationField : ConfigurationField
    {
        public const string ElementTypes = "elementTypes";

        public ElementTypesConfigurationField(IContentTypeService contentTypeService)
        {
            var items = contentTypeService
            .GetAllElementTypes()
            .OrderBy(x => x.Name)
            .Select(x => new DataListItem
            {
                Description = x.Description,
                Icon = x.Icon,
                Name = x.Name,
                Value = x.GetUdi().ToString(),
            });

            Key = ElementTypes;
            Name = "Element types";
            Description = "Select the element types to use.";
            View = IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorViewPath);
            Config = new Dictionary<string, object>
            {
                { AllowDuplicatesConfigurationField.AllowDuplicates, Constants.Values.False },
                { ItemPickerConfigurationEditor.Items, items },
                { ItemPickerTypeConfigurationField.ListType, ItemPickerTypeConfigurationField.List },
                { ItemPickerConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorOverlayViewPath) },
                { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.False },
            };
        }
    }
}
