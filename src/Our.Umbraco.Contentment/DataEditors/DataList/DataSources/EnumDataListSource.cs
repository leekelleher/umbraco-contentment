/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class EnumDataListSource : IDataListSource
    {
        public string Name => "Enum";

        public string Description => "Select an enum from a .NET assembly as the data source.";

        public string Icon => "icon-indent";

        [ConfigurationField(typeof(EnumTypeConfigurationField))]
        public string EnumType { get; set; }

        [ConfigurationField("sortAlphabetically", "Sort Alphabetically", "boolean", Description = "Select to sort the enum in alphabetical order. The default order is defined by the enum itself.")]
        public string SortAlphabetically { get; set; } // TODO: I had this as a `bool`, but JSON.NET can't deserialize the string "1" to a bool. Look at making a converter.

        public Dictionary<string, string> GetItems()
        {
            // TODO: Review this, make it bulletproof

            var type = TypeFinder.GetTypeByName(EnumType);
            var names = Enum.GetNames(type);

            if (SortAlphabetically.TryConvertTo<bool>().Result)
            {
                Array.Sort(names, StringComparer.InvariantCultureIgnoreCase);
            }

            return names.ToDictionary(x => x, x => x.SplitPascalCasing());
        }

        class EnumTypeConfigurationField : ConfigurationField
        {
            public EnumTypeConfigurationField()
            {
                var items = new[]
                {
                    typeof(global::Umbraco.Core.Persistence.DatabaseModelDefinitions.Direction),
                    typeof(global::Umbraco.Core.Logging.LogLevel),
                    typeof(global::Umbraco.Web.Models.ContentEditing.UmbracoEntityTypes),
                }.Select(x => new { label = x.Name.SplitPascalCasing(), value = x.GetFullNameWithAssembly() });

                Key = "enumType";
                Name = "Enum";
                // TODO: Figure out how to develop this editor type.
                Description = "TODO: This field will become an assembly/type discovery editor, to locate the enum.";
                View = IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    { "allowEmpty", Constants.Values.False },
                    { Constants.Conventions.ConfigurationEditors.Items, items }
                };
            }
        }
    }
}
