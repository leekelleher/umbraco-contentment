/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ItemPickerTypeConfigurationField : ConfigurationField
    {
        internal const string ListType = "listType";
        internal const string Grid = "grid";
        internal const string List = "list";

        public ItemPickerTypeConfigurationField()
            : this(List)
        { }

        public ItemPickerTypeConfigurationField(string defaultValue)
            : base()
        {
            Key = ListType;
            Name = "List type";
            Description = "Select the style of list to be displayed in the overlay.";
            View = IOHelper.ResolveUrl(RadioButtonListDataEditor.DataEditorViewPath);
            Config = new Dictionary<string, object>
            {
                { RadioButtonListConfigurationEditor.Items, new[]
                    {
                        new DataListItem { Name = nameof(Grid), Value = Grid, Description = "Grid displays as a card based layout, (4 cards per row)." },
                        new DataListItem { Name = nameof(List), Value = List, Description = "List will display as a menu of single items." }
                    }
                },
                { RadioButtonListConfigurationEditor.DefaultValue, defaultValue }
            };
        }
    }
}
