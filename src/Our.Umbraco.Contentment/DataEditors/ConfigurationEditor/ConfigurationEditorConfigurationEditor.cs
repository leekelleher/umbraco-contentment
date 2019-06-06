/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
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
        public const string DisableSorting = Constants.Conventions.ConfigurationEditors.DisableSorting;
        public const string EnableFilter = "enableFilter";
        public const string HideLabel = Constants.Conventions.ConfigurationEditors.HideLabel;
        public const string Items = Constants.Conventions.ConfigurationEditors.Items;
        public const string MaxItems = Constants.Conventions.ConfigurationEditors.MaxItems;
        public const string OrderBy = "orderBy";
        public const string OverlaySize = "overlaySize";

        public ConfigurationEditorConfigurationEditor()
            : base()
        {
            var configEditors = GetConfigurationEditors<IConfigurationEditorItem>(ignoreFields: true);
            var items = new List<DataListItem>();
            foreach (var configEditor in configEditors)
            {
                items.Add(new DataListItem
                {
                    Icon = configEditor.Icon,
                    Name = configEditor.Name,
                    Description = configEditor.Description,
                    Value = configEditor.Type
                });
            }

            Fields.Add(
                Items,
                nameof(Items),
                "Select the configuration editors to use.",
                IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorViewPath),
                new Dictionary<string, object>
                {
                    { ItemPickerConfigurationEditor.Items, items },
                    { ItemPickerConfigurationEditor.AllowDuplicates, Constants.Values.False }
                });

            Fields.Add(
                EnableFilter,
                "Enable search filter?",
                "Select to enable the search filter in the overlay selection panel.",
                "boolean");

            Fields.Add(new OverlaySizeConfigurationField());
            Fields.AddMaxItems();
            Fields.AddDisableSorting();
            Fields.AddHideLabel();
        }

        internal class OverlaySizeConfigurationField : ConfigurationField
        {
            public const string Small = "small";
            public const string Large = "large";

            public OverlaySizeConfigurationField()
                : base()
            {
                var items = new[]
                {
                    new { name = nameof(Small), value = Small },
                    new { name = nameof(Large), value = Large }
                };

                Key = OverlaySize;
                Name = "Overlay size";
                Description = "Select the size of the overlay editing panel. By default this is set to 'large'. However if the configuration editor fields require a smaller panel, select 'small'.";
                View = IOHelper.ResolveUrl(RadioButtonListDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    { RadioButtonListConfigurationEditor.Orientation, RadioButtonListConfigurationEditor.OrientationConfigurationField.Horizontal },
                    { RadioButtonListConfigurationEditor.Items, items },
                    { RadioButtonListConfigurationEditor.DefaultValue, Large }
                };
            }
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValue(Items, out var items) && items is JArray array && array.Count > 0)
            {
                var types = new List<Type>();

                foreach (var item in array)
                {
                    var type = TypeFinder.GetTypeByName(item.Value<string>());
                    if (type != null)
                    {
                        types.Add(type);
                    }
                }

                config[Items] = GetConfigurationEditors<IConfigurationEditorItem>(types);
                config[OrderBy] = string.Empty;
            }

            return config;
        }

        // TODO: Review if these methods should be in a "Service" or other class? Feels odd them being in here. [LK]
        private static IEnumerable<ConfigurationEditorModel> GetConfigurationEditors<TConfigurationEditor>(IEnumerable<Type> types, bool ignoreFields = false)
            where TConfigurationEditor : class, IConfigurationEditorItem
        {
            if (types == null)
                return Array.Empty<ConfigurationEditorModel>();

            var models = new List<ConfigurationEditorModel>();

            foreach (var type in types)
            {
                var provider = Activator.CreateInstance(type) as TConfigurationEditor;
                if (provider == null)
                    continue;

                var fields = new List<ConfigurationField>();

                if (ignoreFields == false)
                {
                    var properties = type.GetProperties();

                    foreach (var property in properties)
                    {
                        if (Attribute.IsDefined(property, typeof(ConfigurationFieldAttribute)) == false)
                            continue;

                        var attr = property.GetCustomAttribute<ConfigurationFieldAttribute>(false);
                        if (attr == null)
                            continue;

                        if (attr.Type != null)
                        {
                            var field = Activator.CreateInstance(attr.Type) as ConfigurationField;
                            if (field != null)
                            {
                                fields.Add(field);
                            }
                        }
                        else
                        {
                            fields.Add(new ConfigurationField
                            {
                                Key = attr.Key ?? property.Name,
                                Name = attr.Name ?? property.Name,
                                PropertyName = property.Name,
                                PropertyType = property.PropertyType,
                                Description = attr.Description,
                                HideLabel = attr.HideLabel,
                                View = attr.View
                            });
                        }
                    }
                }

                models.Add(new ConfigurationEditorModel
                {
                    Type = type.GetFullNameWithAssembly(),
                    Name = provider.Name ?? type.Name.SplitPascalCasing(),
                    Description = provider.Description,
                    Icon = provider.Icon ?? "icon-science",
                    Fields = fields
                });
            }

            return models;
        }

        internal static IEnumerable<ConfigurationEditorModel> GetConfigurationEditors<TConfigurationEditor>(bool ignoreFields = false)
            where TConfigurationEditor : class, IConfigurationEditorItem
        {
            // TODO: [LK:2019-06-06] Replace `Current.TypeLoader` using DI.
            return GetConfigurationEditors<TConfigurationEditor>(Current.TypeLoader.GetTypes<TConfigurationEditor>(), ignoreFields);
        }
    }
}
