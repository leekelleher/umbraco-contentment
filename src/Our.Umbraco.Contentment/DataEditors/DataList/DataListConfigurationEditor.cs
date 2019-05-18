/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class DataListConfigurationEditor : ConfigurationEditor
    {
        public DataListConfigurationEditor()
            : base()
        {
            var dataSources = TypeFinder
                .FindClassesOfType<IDataProvider>()
                .Select(t =>
                {
                    var provider = Activator.CreateInstance(t) as IDataProvider;

                    var fields = t
                        .GetProperties()
                        .Where(x => Attribute.IsDefined(x, typeof(ConfigurationFieldAttribute)))
                        .Select(x =>
                        {
                            var attr = x.GetCustomAttribute<ConfigurationFieldAttribute>(false);
                            if (attr?.Type != null)
                            {
                                return Activator.CreateInstance(attr.Type) as ConfigurationField;
                            }

                            return new ConfigurationField
                            {
                                Key = attr?.Key ?? x.Name,
                                Name = attr?.Name ?? x.Name,
                                PropertyName = x.Name,
                                PropertyType = x.PropertyType,
                                Description = attr?.Description,
                                HideLabel = attr?.HideLabel ?? false,
                                View = attr?.View
                            };
                        });

                    return new
                    {
                        type = t.GetFullNameWithAssembly(),
                        name = provider?.Name ?? t.Name.SplitPascalCasing(),
                        description = provider?.Description,
                        icon = provider?.Icon ?? "icon-science",
                        fields
                    };
                })
                .ToArray();

            Fields.Add(
                "dataSource",
                "Data Source",
                "Select and configure the data source provider.",
                IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorViewPath),
                new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationEditors.Items, dataSources },
                    { Constants.Conventions.ConfigurationEditors.MaxItems, 1 },
                    { Constants.Conventions.ConfigurationEditors.DisableSorting, Constants.Values.False }
                });

            var listTypes = new[]
            {
                new { label = "Checkbox List", value = "checkboxlist" },
                new { label = "Dropdown List", value = IOHelper.ResolveUrl(DropdownDataEditor.DataEditorViewPath) },
                new { label = "Radio Button List", value = "radiobuttons" },
            };

            Fields.Add(
                "listType",
                "List Type",
                "[Add a friendly description]",
                IOHelper.ResolveUrl(DropdownDataEditor.DataEditorViewPath),
                new Dictionary<string, object> {
                     { "allowEmpty", 0 },
                    { Constants.Conventions.ConfigurationEditors.Items, listTypes }
                });

            Fields.AddHideLabel();
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            // TODO: If it's an Umbraco control, then we need to manipulate the "items", so that they are in `{ "key", "value" }` format
            // listType
            // NOTE: [LK] I got up to here. I am populating umbraco editors (checkboxlist, radiobuttonlist) with the data, but the format isn't quite right.
            // I wonder whether having a dropdown for "listType", we make it selectable/configurable, like the "dataSource"?

            if (config.TryGetValue("dataSource", out var dataSource) && dataSource is JArray array && array.Count > 0)
            {
                // TODO: Check whether to do this here, or in the ValueEditor class?

                // TODO: Review this, make it bulletproof

                var item = array[0];
                var type = TypeFinder.GetTypeByName(item["type"].ToString());
                if (type != null)
                {
                    var provider = item["value"].ToObject(type) as IDataProvider;
                    var options = provider?.GetItems() ?? new Dictionary<string, string>();

                    config.Add("items", options.Select(x => new { label = x.Value, value = x.Key }));
                }

                config.Remove("dataSource");
            }

            return config;
        }
    }
}
