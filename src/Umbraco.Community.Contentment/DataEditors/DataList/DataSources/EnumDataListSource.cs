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

        public OverlaySize OverlaySize => OverlaySize.Small;

        public Dictionary<string, object> DefaultValues => default;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "enumType",
                Name = "Enumeration type",
                Description = "Select the enumeration from an assembly type.",
                View = CascadingDropdownListDataEditor.DataEditorViewPath,
                Config = new Dictionary<string, object>
                {
                    { CascadingDropdownListDataEditor.APIs, new[]
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
            var type = GetValueType(config);
            if (type == null)
            {
                return Enumerable.Empty<DataListItem>();
            }

            var items = new List<DataListItem>();

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

            if (config.TryGetValueAs("sortAlphabetically", out bool boolean) == true && boolean == true)
            {
                return items.OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
            }

            return items;
        }

        public Type GetValueType(Dictionary<string, object> config)
        {
            if (config.TryGetValueAs("enumType", out JArray array) == true)
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
                        if (type != null && type.IsEnum == true)
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
                // NOTE: Can't use `Enum.TryParse` here, as it's only available with generic types in .NET 4.8.
                try { return Enum.Parse(type, value, true); } catch (Exception ex) { _logger.Error<EnumDataListSource>(ex); }

                // If the value doesn't match the Enum field, then it is most likely set with `DataListItemAttribute.Value`.
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                foreach (var field in fields)
                {
                    var attr = field.GetCustomAttribute<DataListItemAttribute>(false);
                    if (value.InvariantEquals(attr?.Value) == true)
                    {
                        return Enum.Parse(type, field.Name);
                    }
                }

                _logger.Debug<EnumDataListSource>($"Unable to find value '{value}' in enum '{type.FullName}'.");
            }

            return type.GetDefaultValue();
        }
    }
}
