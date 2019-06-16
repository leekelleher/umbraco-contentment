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
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class EnumDataListSource : IDataListSource
    {
        public string Name => "Enum";

        public string Description => "Select an enum from a .NET assembly as the data source.";

        public string Icon => "icon-indent";

        [ConfigurationField(typeof(EnumTypeConfigurationField))]
        public string[] EnumType { get; set; }

        [ConfigurationField(typeof(SortAlphabeticallyConfigurationField))]
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
            public const string Enum = "enumType";

            public EnumTypeConfigurationField()
            {
                var apis = new[]
                {
                    EnumDataSourceApiController.GetAssembliesUrl,
                    EnumDataSourceApiController.GetEnumsUrl,
                };

                Key = Enum;
                Name = nameof(Enum);
                Description = "Select the enum from an assembly type.";
                View = IOHelper.ResolveUrl(CascadingDropdownListDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    { CascadingDropdownListConfigurationEditor.APIs, apis }
                };
            }
        }

        class SortAlphabeticallyConfigurationField : ConfigurationField
        {
            public const string SortAlphabetically = "sortAlphabetically";

            public SortAlphabeticallyConfigurationField()
            {
                Key = SortAlphabetically;
                Name = "Sort alphabetically";
                Description = "Select to sort the enum in alphabetical order.<br>By default, the order is defined by the enum itself.";
                View = "boolean";
            }
        }
    }
}
