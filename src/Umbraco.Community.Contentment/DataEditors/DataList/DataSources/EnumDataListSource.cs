/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Umbraco.Community.Contentment.Web.Controllers;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class EnumDataListSource : IDataListSource, IDataListSourceValueConverter, IContentmentListTemplateItem
    {
        private readonly ConcurrentDictionary<Type, (List<DataListItem>, Dictionary<string, object>)> _lookup;
        private readonly IIOHelper _ioHelper;
        private readonly IShortStringHelper _shortStringHelper;

        private readonly ILogger<EnumDataListSource> _logger;

        public EnumDataListSource(
            ILogger<EnumDataListSource> logger,
            IShortStringHelper shortStringHelper,
            IIOHelper ioHelper)
        {
            _lookup = new ConcurrentDictionary<Type, (List<DataListItem>, Dictionary<string, object>)>();

            _logger = logger;
            _shortStringHelper = shortStringHelper;
            _ioHelper = ioHelper;
        }

        public string Name => ".NET Enumeration";

        public string NameTemplate => default;

        public string Description => "Select an enumeration from a .NET assembly as the data source.";

        public string DescriptionTemplate => "{{ enumType[1] }}";

        public string Icon => "icon-indent";

        public string Group => Constants.Conventions.DataSourceGroups.DotNet;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public Dictionary<string, object> DefaultValues => default;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "enumType",
                Name = "Enumeration type",
                Description = "Select the enumeration from an assembly type.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(CascadingDropdownListDataEditor.DataEditorViewPath),
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

        private (List<DataListItem>, Dictionary<string, object>) EnumValueFactory(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            var items = new List<DataListItem>(fields.Length);
            var values = new Dictionary<string, object>(fields.Length);

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<DataListItemAttribute>(false);
                if (attr?.Ignore == true)
                {
                    continue;
                }

                var attr2 = field.GetCustomAttribute<DescriptionAttribute>(false);
                var attr3 = field.GetCustomAttribute<EnumMemberAttribute>(false);
                var value = attr3?.Value ?? attr?.Value ?? field.Name;

                items.Add(new DataListItem
                {
                    Description = attr?.Description ?? attr2?.Description,
                    Disabled = attr?.Disabled ?? false,
                    Icon = attr?.Icon,
                    Name = attr?.Name ?? field.Name.SplitPascalCasing(_shortStringHelper),
                    Value = attr3?.Value ?? attr?.Value ?? field.Name
                });

                values.Add(value, Enum.Parse(type, field.Name));
            }

            return (items, values);
        }

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var type = GetValueType(config);

            if (type == null)
            {
                return Enumerable.Empty<DataListItem>();
            }

            var entry = _lookup.GetOrAdd(type, EnumValueFactory);

            if (entry.Item1 == null)
            {
                return Enumerable.Empty<DataListItem>();
            }

            var items = entry.Item1;

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
                    try
                    {
                        assembly = Assembly.Load(enumType[0]);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unable to load target type.");
                    }

                    if (assembly != null)
                    {
                        var type = default(Type);
                        try
                        {
                            type = assembly.GetType(enumType[1]);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Unable to retrieve target type.");
                        }

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
                var entry = _lookup.GetOrAdd(type, EnumValueFactory);
                if (entry.Item2 != null && entry.Item2.TryGetValue(value, out var enumValue) == true)
                {
                    return enumValue;
                }

                // NOTE: Can't use `Enum.TryParse` here, as it's only available with generic types in .NET 4.8.
                try { return Enum.Parse(type, value, true); } catch { /* ¯\_(ツ)_/¯ */ }

                _logger.LogDebug($"Unable to find value '{value}' in enum '{type.FullName}'.");
            }

            return type.GetDefaultValue();
        }
    }
}
