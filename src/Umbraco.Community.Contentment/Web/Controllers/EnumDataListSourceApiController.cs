/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Umbraco.Community.Contentment.DataEditors;
#if NET472
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Strings;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
#else
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;
#endif

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
#if NET472
            const string App_Code = "App_Code";
#endif

            var options = new SortedSet<string>(StringComparer.InvariantCultureIgnoreCase);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (assemblies?.Length > 0)
            {
                foreach (var assembly in assemblies)
                {
                    var assemblyName = assembly.GetName();
                    if (options.Contains(assemblyName.Name) == true || assembly.IsDynamic == true)
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

#if NET472
                    if (assembly.FullName.StartsWith(App_Code) == true && options.Contains(App_Code) == false)
                    {
                        options.Add(App_Code);
                    }
                    else
#endif
                    {
                        options.Add(assemblyName.Name);
                    }
                }
            }

            return options.Select(x => new DataListItem { Name = x, Value = x });
        }

        [HttpGet]
        public IEnumerable<DataListItem> GetEnums(string assembly)
        {
            var options = new SortedDictionary<string, DataListItem>();

            var types = Assembly.Load(assembly).GetTypes();

            foreach (var type in types)
            {
                if (type.IsEnum == false)
                {
                    continue;
                }

                options.Add(type.FullName, new DataListItem { Name = type.Name.SplitPascalCasing(_shortStringHelper), Value = type.FullName });
            }

            return options.Values;
        }
    }
}
