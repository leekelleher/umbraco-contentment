/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Web;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.Contentment.DataEditors
{
#if !DEBUG
    // TODO: IsWorkInProgress - Under development.
    [HideFromTypeFinder]
#endif
    internal class EnumDataListSource : IDataListSource
    {
        public string Name => "Enum";

        public string Description => "Select an enum from a .NET assembly as the data source.";

        public string Icon => "icon-indent";

        [ConfigurationField(typeof(EnumTypeConfigurationField))]
        public string[] EnumType { get; set; }

        [ConfigurationField("sortAlphabetically", "Sort Alphabetically", "boolean", Description = "Select to sort the enum in alphabetical order. The default order is defined by the enum itself.")]
        public bool SortAlphabetically { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            var assembly = default(Assembly);
            try { assembly = Assembly.Load(EnumType[0]); } catch (Exception ex) { Current.Logger.Error<EnumDataListSource>(ex); }
            if (assembly == null)
                return Enumerable.Empty<DataListItem>();

            var enumType = default(Type);
            try { enumType = assembly.GetType(EnumType[1]); } catch (Exception ex) { Current.Logger.Error<EnumDataListSource>(ex); }
            if (enumType == null)
                return Enumerable.Empty<DataListItem>();

            var names = default(string[]);
            try { names = Enum.GetNames(enumType); } catch (Exception ex) { Current.Logger.Error<EnumDataListSource>(ex); }
            if (names == null)
                return Enumerable.Empty<DataListItem>();

            if (SortAlphabetically)
            {
                Array.Sort(names, StringComparer.InvariantCultureIgnoreCase);
            }

            return names.Select(x => new DataListItem
            {
                Name = x.SplitPascalCasing(),
                Value = x
            });
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
                    { "apis", apis }
                };
            }
        }
    }

    // TODO: [LK:2019-06-06] Review where to put this controller. Better namespace?
    [PluginController("Contentment"), IsBackOffice]
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
