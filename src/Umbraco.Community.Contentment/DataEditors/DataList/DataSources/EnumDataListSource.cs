/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Community.Contentment.Web.Controllers;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class EnumDataListSource : IDataListSource
    {
        public string Name => ".NET Enumeration";

        public string Description => "Select an enumeration from a .NET assembly as the data source.";

        public string Icon => "icon-indent";

        public Dictionary<string, object> DefaultValues => default;

        [ConfigurationField(typeof(EnumTypeConfigurationField))]
        public string[] EnumType { get; set; }

        [ConfigurationField(typeof(SortAlphabeticallyConfigurationField))]
        public bool SortAlphabetically { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            var items = new List<DataListItem>();

            if (EnumType == null || EnumType.Length < 2)
                return items;

            var assembly = default(Assembly);
            try { assembly = Assembly.Load(EnumType[0]); } catch (Exception ex) { Current.Logger.Error<EnumDataListSource>(ex); }
            if (assembly == null)
                return items;

            var enumType = default(Type);
            try { enumType = assembly.GetType(EnumType[1]); } catch (Exception ex) { Current.Logger.Error<EnumDataListSource>(ex); }
            if (enumType == null || enumType.IsEnum == false)
                return items;

            var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<DataListItemAttribute>(false);
                items.Add(new DataListItem
                {
                    Description = attr?.Description,
                    Icon = attr?.Icon,
                    Name = attr?.Name ?? field.Name.SplitPascalCasing(),
                    Value = field.Name
                });
            }

            if (SortAlphabetically)
            {
                return items.OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
            }

            return items;
        }

        private class EnumTypeConfigurationField : ConfigurationField
        {
            internal const string EnumType = "enumType";

            public EnumTypeConfigurationField()
            {
                Key = EnumType;
                Name = "Enumeration type";
                Description = "Select the enumeration from an assembly type.";
                View = IOHelper.ResolveUrl(CascadingDropdownListDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    { CascadingDropdownListConfigurationEditor.APIs, new[]
                        {
                            EnumDataSourceApiController.GetAssembliesUrl,
                            EnumDataSourceApiController.GetEnumsUrl,
                        }
                    }
                };
            }
        }

        class SortAlphabeticallyConfigurationField : ConfigurationField
        {
            internal const string SortAlphabetically = "sortAlphabetically";

            public SortAlphabeticallyConfigurationField()
            {
                Key = SortAlphabetically;
                Name = "Sort alphabetically?";
                Description = "Select to sort the enumeration in alphabetical order.<br>By default, the order is defined by the enumeration itself.";
                View = "boolean";
            }
        }
    }
}
