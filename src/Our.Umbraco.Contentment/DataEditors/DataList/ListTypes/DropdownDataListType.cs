/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class DropdownDataListType : IDataListType
    {
        public string Name => "Dropdown List";

        public string Description => "[Add a friendly description]";

        public string Icon => "icon-list";

        public string View => IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath);

        [ConfigurationField(typeof(AllowEmptyConfigurationField))]
        public string AllowEmpty { get; set; } // TODO: I had this as a `bool`, but JSON.NET can't deserialize the string "1" to a bool. Look at making a converter.

        class AllowEmptyConfigurationField : ConfigurationField
        {
            public AllowEmptyConfigurationField()
            {
                Key = "allowEmpty";
                Name = "Allow Empty";
                Description = "Enable to allow an empty option at the top of the dropdown list.";
                View = "views/propertyeditors/boolean/boolean.html";
                Config = new Dictionary<string, object>
                {
                    { "default", Constants.Values.True }
                };
            }
        }
    }
}
