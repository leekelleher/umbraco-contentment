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
                new { name = nameof(Grid), value = Grid },
                new { name = nameof(List), value = List }
            };

            Key = ListType;
            Name = "List type";
            Description = "Select the style of list to be displayed in the overlay.<br><br>Grid displays as a card based layout, (3 cards per row), whereas List will display as a menu of single items.";
            View = IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath);
            Config = new Dictionary<string, object>
            {
                { AllowEmptyConfigurationField.AllowEmpty,  Constants.Values.False },
                { DropdownListConfigurationEditor.Items, items },
                { DropdownListConfigurationEditor.DefaultValue, Grid }
            };
        }
    }
}
