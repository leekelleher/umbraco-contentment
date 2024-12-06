/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Strings;
using Umbraco.Community.Contentment.Api.Management;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class EnumDataListSource : DataListToDataPickerSourceBridge, IDataListSource, IDataSourceValueConverter, IContentmentListTemplateItem
    {
        private readonly ConcurrentDictionary<Type, (List<DataListItem>, Dictionary<string, object>)> _lookup;
        private readonly IShortStringHelper _shortStringHelper;

        public EnumDataListSource(IShortStringHelper shortStringHelper)
        {
            _lookup = new ConcurrentDictionary<Type, (List<DataListItem>, Dictionary<string, object>)>();

            _shortStringHelper = shortStringHelper;
        }

        public override string Name => ".NET Enumeration";

        public string? NameTemplate => default;

        public override string Description => "Select an enumeration from a .NET assembly as the data source.";

        public string? DescriptionTemplate => "{{ enumType[1] }}";

        public override string Icon => "icon-indent";

        public override string Group => Constants.Conventions.DataSourceGroups.DotNet;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override Dictionary<string, object>? DefaultValues => default;

        public override IEnumerable<ContentmentConfigurationField> Fields => new[]
        {
            new ContentmentConfigurationField
            {
                Key = "enumType",
                Name = "Enumeration type",
                Description = "Select the enumeration from an assembly type.",
                PropertyEditorUiAlias = CascadingDropdownListDataEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { CascadingDropdownListDataEditor.APIs, new[]
                        {
                            AssemblyEnumController.GetAssembliesUrl,
                            AssemblyEnumController.GetEnumsUrl,
                        }
                    }
                }
            },
            new ContentmentConfigurationField
            {
                Key = "sortAlphabetically",
                Name = "Sort alphabetically?",
                Description = "Select to sort the enumeration in alphabetical order.<br>By default, the order is defined by the enumeration itself.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
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
                    Group = attr?.Group,
                    Icon = attr?.Icon,
                    Name = attr?.Name ?? field.Name.SplitPascalCasing(_shortStringHelper),
                    Value = attr3?.Value ?? attr?.Value ?? field.Name
                });

                values.Add(value, Enum.Parse(type, field.Name));
            }

            return (items, values);
        }

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
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

        public Type? GetValueType(Dictionary<string, object>? config)
        {
            if (config?.TryGetValueAs("enumType", out List<string>? enumType) == true &&
                enumType?.Count > 1)
            {
                var assembly = default(Assembly);
                try
                {
                    assembly = Assembly.Load(enumType[0]);
                }
                catch (Exception)
                {
                    // Unable to load target type.
                    // Nobody wants an exception here. Wondering if `Assembly.TryLoad()` should be a thing? [LK]
                }

                if (assembly is not null)
                {
                    var type = default(Type);
                    try
                    {
                        type = assembly.GetType(enumType[1]);
                    }
                    catch (Exception)
                    {
                        // Unable to retrieve target type.
                        // Again, what are users going to do about an exception here? `assembly.TryGetType()` anyone? [LK]
                    }

                    if (type?.IsEnum == true)
                    {
                        return type;
                    }
                }
            }

            return typeof(object);
        }

        public object? ConvertValue(Type type, string value)
        {
            if (string.IsNullOrWhiteSpace(value) == false && type.IsEnum == true)
            {
                var entry = _lookup.GetOrAdd(type, EnumValueFactory);
                if (entry.Item2 != null && entry.Item2.TryGetValue(value, out var enumValue) == true)
                {
                    return enumValue;
                }

                // NOTE: Can't use `Enum.TryParse` here, as it's only available with generic types in .NET 4.8.
                try
                { return Enum.Parse(type, value, true); }
                catch { /* ¯\_(ツ)_/¯ */ }
            }

            return type.GetDefaultValue();
        }
    }
}
