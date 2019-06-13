/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.Contentment
{
    [PluginController(Constants.Internals.PluginControllerName), IsBackOffice]
    public class EnumDataSourceApiController : UmbracoAuthorizedJsonController
    {
        private const string App_Code = "App_Code";

        internal const string GetAssembliesUrl = "backoffice/Contentment/EnumDataSourceApi/GetAssemblies";
        internal const string GetEnumsUrl = "backoffice/Contentment/EnumDataSourceApi/GetEnums?assembly={0}";

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

                    if (assembly.FullName.StartsWith(App_Code) && options.ContainsKey(App_Code) == false)
                    {
                        options.Add(App_Code, App_Code);
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
