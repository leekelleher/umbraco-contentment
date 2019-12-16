/* Copyright © 2019 Lee Kelleher and Marc Stöcker.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ImageButtonListConfigurationEditor : ConfigurationEditor
    {
        internal const string Items = "items";
        internal const string DefaultValue = "defaultValue";

        public ImageButtonListConfigurationEditor()
            : base()
        {
            var listFields = new[]
            {
                new ConfigurationField
                {
                    Key = "name",
                    Name = "Name",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "description",
                    Name = "Description",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "value",
                    Name = "Value",
                    View = "textbox"
                }
            };

            Fields.Add(
                Items,
                "Options",
                "Configure the option items for the image button list.",
                IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { DataTableConfigurationEditor.FieldItems, listFields },
                    { MaxItemsConfigurationField.MaxItems, 0 },
                    { DisableSortingConfigurationField.DisableSorting, Constants.Values.False },
                    { DataTableConfigurationEditor.RestrictWidth, Constants.Values.True },
                    { DataTableConfigurationEditor.UsePrevalueEditors, Constants.Values.False }
                });

            Fields.Add(new ShowDescriptionsConfigurationField());
            Fields.Add(new SvgSpriteMediaConfigurationField());
        }
    }
}
