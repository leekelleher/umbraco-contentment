/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Umbraco.Community.Contentment.Web.Controllers;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class EnumDataListSource : IDataListSource, IDataListSourceValueConverter
    {
        private readonly ILogger _logger;

        public EnumDataListSource(ILogger logger)
        {
            _logger = logger;
        }

        public string Name => ".NET Enumeration";

        public string Description => "Select an enumeration from a .NET assembly as the data source.";

        public string Icon => "icon-indent";

        public Dictionary<string, object> DefaultValues => default;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "enumType",
                Name = "Enumeration type",
                Description = "Select the enumeration from an assembly type.",
                View = IOHelper.ResolveUrl(CascadingDropdownListDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { CascadingDropdownListConfigurationEditor.APIs, new[]
                        {
                            EnumDataSourceApiController.GetAssembliesUrl,
                            EnumDataSourceApiController.GetEnumsUrl,
                        }
                    }
                }
            },
            new ConfigurationField
            {
                Key = "sortAlphabetically",
                Name = "Sort alphabetically?",
                Description = "Select to sort the enumeration in alphabetical order.<br>By default, the order is defined by the enumeration itself.",
                View = "boolean"
            }
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var items = new List<DataListItem>();

            var type = GetValueType(config);
            if (type == null)
            {
                return items;
            }

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<DataListItemAttribute>(false);
                if (attr?.Ignore == true)
                {
                    continue;
                }

                var attr2 = field.GetCustomAttribute<DescriptionAttribute>(false);
                items.Add(new DataListItem
                {
                    Description = attr?.Description ?? attr2?.Description,
                    Disabled = attr?.Disabled ?? false,
                    Icon = attr?.Icon,
                    Name = attr?.Name ?? field.Name.SplitPascalCasing(),
                    Value = attr?.Value ?? field.Name
                });
            }

            if (config.TryGetValueAs("sortAlphabetically", out string boolean) && boolean.Equals("1"))
            {
                return items.OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
            }

            return items;
        }

        public Type GetValueType(Dictionary<string, object> config)
        {
            if (config.TryGetValueAs("enumType", out JArray array))
            {
                var enumType = array.ToObject<string[]>();
                if (enumType?.Length > 1)
                {
                    var assembly = default(Assembly);
                    try { assembly = Assembly.Load(enumType[0]); } catch (Exception ex) { _logger.Error<EnumDataListSource>(ex); }
                    if (assembly != null)
                    {
                        var type = default(Type);
                        try { type = assembly.GetType(enumType[1]); } catch (Exception ex) { _logger.Error<EnumDataListSource>(ex); }
                        if (type != null && type.IsEnum)
                        {
                            return type;
                        }
                    }
                }
            }

            return null;
        }

        public object ConvertValue(Type type, string value)
        {
            if (string.IsNullOrWhiteSpace(value) == false && type?.IsEnum == true)
            {
                try { return Enum.Parse(type, value, true); } catch (Exception ex) { _logger.Error<EnumDataListSource>(ex); }
            }

            return type.GetDefaultValue();
        }
    }
}
