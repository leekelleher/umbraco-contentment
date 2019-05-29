/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class EnumDataListSource : IDataListSource
    {
        public string Name => "Enum";

        public string Description => "Select an enum from a .NET assembly as the data source.";

        public string Icon => "icon-indent";

        [ConfigurationField(typeof(EnumTypeConfigurationField))]
        public string[] EnumType { get; set; }

        [ConfigurationField("sortAlphabetically", "Sort Alphabetically", "boolean", Description = "Select to sort the enum in alphabetical order. The default order is defined by the enum itself.")]
        public bool SortAlphabetically { get; set; }

        public IEnumerable<DataListItemModel> GetItems()
        {
            // TODO: Review this, make it bulletproof

            // TODO: What to do if the enum no longer exists? (for whatever reason?) [LK]
            var assembly = Assembly.Load(EnumType[0]);
            var enumType = assembly.GetType(EnumType[1]);
            var names = Enum.GetNames(enumType);

            if (SortAlphabetically)
            {
                Array.Sort(names, StringComparer.InvariantCultureIgnoreCase);
            }

            return names.Select(x => new DataListItemModel {
                Icon = this.Icon,
                Name = x.SplitPascalCasing(),
                Value = x
            });
            //.ToDictionary(x => x, x => x.SplitPascalCasing());
        }

        class EnumTypeConfigurationField : ConfigurationField
        {
            public EnumTypeConfigurationField()
            {
                var apis = new[]
                {
                    "backoffice/Contentment/EnumDataListSourceApi/GetAssemblies",
                    "backoffice/Contentment/EnumDataListSourceApi/GetEnums?assembly={0}",
                };

                Key = "enumType";
                Name = "Enum";
                Description = "Select the enum from an assembly type.";
                View = IOHelper.ResolveUrl(CascadingDropdownListDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    { "apis", apis },
                    // TODO: We can send down the initial options (assemblies) along with the config, save on an extra HTTP request. [LK]
                    //{ "options", Array.Empty<object>() }
                };
            }
        }
    }

    [PluginController("Contentment")]
    public class EnumDataListSourceApiController : UmbracoAuthorizedJsonController
    {
        public IEnumerable<object> GetAssemblies()
        {
            var options = new SortedDictionary<string, string>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (assemblies?.Length > 0)
            {
                foreach (var assembly in assemblies)
                {
                    if (options.ContainsKey(assembly.FullName) || assembly.IsDynamic || assembly.ExportedTypes?.Any(x => x.IsEnum) == false)
                        continue;

                    if (assembly.FullName.StartsWith("App_Code") && options.ContainsKey("App_Code") == false)
                    {
                        options.Add("App_Code", "App_Code");
                    }
                    else
                    {
                        var assemblyName = assembly.GetName();
                        options.Add(assemblyName.FullName, assemblyName.Name);
                    }
                }
            }

            return options.Select(x => new { name = x.Value, value = x.Key });
        }

        public IEnumerable<object> GetEnums(string assembly)
        {
            return Assembly
                .Load(assembly)
                .GetTypes()
                .Where(x => x.IsEnum)
                .Select(x => new { name = x.Name.SplitPascalCasing(), value = x.FullName })
                .OrderBy(x => x.name);
        }
    }
}
