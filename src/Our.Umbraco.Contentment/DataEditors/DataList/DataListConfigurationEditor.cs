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
            var dataSources = GetDataSources();
            var listTypes = GetListTypes();

            Fields.Add(
                "dataSource",
                "Data Source",
                "Select and configure the data source.",
                IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorViewPath),
                new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationEditors.Items, dataSources },
                    { Constants.Conventions.ConfigurationEditors.MaxItems, 1 },
                    { Constants.Conventions.ConfigurationEditors.DisableSorting, Constants.Values.True },
                    { "overlaySize", "large" },
#if DEBUG
                    { "debug", Constants.Values.True },
#endif
                });
            Fields.Add(
                "listType",
                "List Type",
                "Select and configure the type of data list.",
                IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorViewPath),
                new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationEditors.Items, listTypes },
                    { Constants.Conventions.ConfigurationEditors.MaxItems, 1 },
                    { Constants.Conventions.ConfigurationEditors.DisableSorting, Constants.Values.True },
                    { "overlaySize", "large" },
#if DEBUG
                    { "debug", Constants.Values.True },
#endif
                });
            Fields.AddHideLabel();
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValue("dataSource", out var dataSource) && dataSource is JArray array && array.Count > 0)
            {
                // TODO: Review this, make it bulletproof

                var item = array[0];
                var type = TypeFinder.GetTypeByName(item["type"].ToString());
                if (type != null)
                {
                    var source = item["value"].ToObject(type) as IDataListSource;
                    var options = source?.GetItems() ?? new Dictionary<string, string>();

                    config.Add("items", options.Select(x => new { label = x.Value, value = x.Key }));
                }

                config.Remove("dataSource");
            }

            if (config.ContainsKey("listType"))
            {
                config.Remove("listType");
            }

            return config;
        }

        private ConfigurationEditorModel[] GetDataSources()
        {
            return GetConfigurationEditors<IDataListSource>();
        }

        private ConfigurationEditorModel[] GetListTypes()
        {
            return GetConfigurationEditors<IDataListType>();
        }

        private ConfigurationEditorModel[] GetConfigurationEditors<TConfigurationEditor>()
            where TConfigurationEditor : class, IConfigurationEditorItem
        {
            return TypeFinder
                .FindClassesOfType<TConfigurationEditor>()
                .Select(t =>
                {
                    var provider = Activator.CreateInstance(t) as TConfigurationEditor;

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

                    return new ConfigurationEditorModel
                    {
                        Type = t.GetFullNameWithAssembly(),
                        Name = provider?.Name ?? t.Name.SplitPascalCasing(),
                        Description = provider?.Description,
                        Icon = provider?.Icon ?? "icon-science",
                        Fields = fields
                    };
                })
                .ToArray();
        }
    }
}
