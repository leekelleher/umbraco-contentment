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
    public class ConfigurationEditorConfigurationEditor : ConfigurationEditor
    {
        public ConfigurationEditorConfigurationEditor()
            : base()
        {
            Fields.Add(
                Constants.Conventions.ConfigurationEditors.Items,
                "Items",
                "[Add a friendly description]", // TODO: Add a friendly description. [LK]
                "views/propertyeditors/multipletextbox/multipletextbox.html",
                new Dictionary<string, object>
                {
                    { "min", "0" },
                    { "max", "0" },
                });

            Fields.Add(
                "enableSearch",
                "Enable search filter?",
                "[Add a friendly description]", // TODO: Add a friendly description [LK]
                "boolean");

            Fields.Add(
                "overlaySize",
                "Overlay size",
                "[Add a friendly description]", // TODO: Add a friendly description. [LK]
                IOHelper.ResolveUrl(RadioButtonListDataEditor.DataEditorViewPath),
                new Dictionary<string, object>
                {
                    {
                        Constants.Conventions.ConfigurationEditors.Items, new[]
                        {
                            new { label = "Small", value = "small" },
                            new { label = "Large", value = "large" }
                        }
                    },
                    { Constants.Conventions.ConfigurationEditors.DefaultValue, "large" },
                    { "orientation", "horizontal" }
                });

            Fields.AddMaxItems();
            Fields.AddDisableSorting();
            Fields.AddHideLabel();
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValue("items", out var items) && items is JArray array && array.Count > 0)
            {
                var types = array
                    .Select(x => x["value"].ToString())
                    .Select(TypeFinder.GetTypeByName)
                    .WhereNotNull();

                var editors = GetConfigurationEditors<IConfigurationEditorItem>(types);

                config["items"] = editors;
            }

            return config;
        }

        private static ConfigurationEditorModel[] GetConfigurationEditors<TConfigurationEditor>(IEnumerable<Type> types)
            where TConfigurationEditor : class, IConfigurationEditorItem
        {
            if (types == null)
                return null;

            return types.Select(t =>
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

        internal static ConfigurationEditorModel[] GetConfigurationEditors<TConfigurationEditor>()
            where TConfigurationEditor : class, IConfigurationEditorItem
        {
            var types = TypeFinder.FindClassesOfType<TConfigurationEditor>();
            return GetConfigurationEditors<TConfigurationEditor>(types);
        }
    }
}
