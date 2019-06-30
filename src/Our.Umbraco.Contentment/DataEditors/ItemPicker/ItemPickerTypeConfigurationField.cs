/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class ItemPickerTypeConfigurationField : ConfigurationField
    {
        public const string ListType = "listType";
        public const string Grid = "grid";
        public const string List = "list";

        public ItemPickerTypeConfigurationField()
            : base()
        {
            var items = new[]
            {
                new DataListItem { Name = nameof(Grid), Value = Grid, Description = "Grid displays as a card based layout, (3 cards per row)." },
                new DataListItem { Name = nameof(List), Value = List, Description = "List will display as a menu of single items." }
            };

            Key = ListType;
            Name = "List type";
            Description = "Select the style of list to be displayed in the overlay.";
            View = IOHelper.ResolveUrl(RadioButtonListDataEditor.DataEditorViewPath);
            Config = new Dictionary<string, object>
            {
                { OrientationConfigurationField.Orientation, OrientationConfigurationField.Vertical },
                { RadioButtonListConfigurationEditor.Items, items },
                { RadioButtonListConfigurationEditor.DefaultValue, Grid }
            };
        }
    }
}
