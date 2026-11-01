/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Web.Controllers
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [PluginController(Constants.Internals.PluginControllerName), IsBackOffice]
    public sealed class EnumDataSourceApiController : UmbracoAuthorizedJsonController
    {
        internal const string GetAssembliesUrl = "backoffice/Contentment/EnumDataSourceApi/GetAssemblies";
        internal const string GetEnumsUrl = "backoffice/Contentment/EnumDataSourceApi/GetEnums?assembly={0}";

        private readonly IShortStringHelper _shortStringHelper;

        public EnumDataSourceApiController(IShortStringHelper shortStringHelper)
        {
            _shortStringHelper = shortStringHelper;
        }

        [HttpGet]
        public IEnumerable<DataListItem> GetAssemblies()
        {
            var options = GetAssemblyNames();

            return options.Select(x => new DataListItem { Name = x, Value = x });
        }

        [HttpGet]
        public IActionResult GetEnums(string assembly)
        {
            // NOTE: Only assemblies that are offered by the `GetAssemblies` endpoint can be loaded.
            if (string.IsNullOrWhiteSpace(assembly) == true || GetAssemblyNames().Contains(assembly) == false)
            {
                return BadRequest("Unable to find the assembly.");
            }

            var options = new SortedDictionary<string, DataListItem>();

            Type[] types;

            try
            {
                types = Assembly.Load(assembly).GetTypes();
            }
            catch (Exception ex) when (ex is FileLoadException or FileNotFoundException or BadImageFormatException or ReflectionTypeLoadException)
            {
                return BadRequest("Unable to load the assembly.");
            }

            foreach (var type in types)
            {
                if (type.IsEnum == false || string.IsNullOrWhiteSpace(type.FullName) == true)
                {
                    continue;
                }

                options.Add(type.FullName, new DataListItem { Name = type.Name.SplitPascalCasing(_shortStringHelper), Value = type.FullName });
            }

            return Ok(options.Values);
        }

        private static SortedSet<string> GetAssemblyNames()
        {
            var options = new SortedSet<string>(StringComparer.InvariantCultureIgnoreCase);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (assemblies?.Length > 0)
            {
                foreach (var assembly in assemblies)
                {
                    var assemblyName = assembly.GetName();
                    if (string.IsNullOrWhiteSpace(assemblyName.Name) == true ||
                        options.Contains(assemblyName.Name) == true ||
                        assembly.IsDynamic == true)
                    {
                        continue;
                    }

                    var hasEnums = false;
                    try
                    {
                        var exportedTypes = assembly.GetExportedTypes();
                        if (exportedTypes != null)
                        {
                            foreach (var exportedType in exportedTypes)
                            {
                                if (exportedType.IsEnum == true)
                                {
                                    hasEnums = true;
                                    break;
                                }
                            }
                        }
                    }
                    catch (FileLoadException) { /* (╯°□°）╯︵ ┻━┻ */ }
                    catch (TypeLoadException) { /* ¯\_(ツ)_/¯ */ }

                    if (hasEnums == false)
                    {
                        continue;
                    }

                    _ = options.Add(assemblyName.Name);
                }
            }

            return options;
        }
    }
}
