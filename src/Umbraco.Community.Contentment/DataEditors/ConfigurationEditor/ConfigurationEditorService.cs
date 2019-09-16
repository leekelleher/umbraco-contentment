/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.PropertyEditors;
using UmbIcons = Umbraco.Core.Constants.Icons;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class ConfigurationEditorService
    {
        public IEnumerable<ConfigurationEditorModel> GetConfigurationEditors<TContentmentListItem>(
            IEnumerable<Type> types,
            bool onlyPublic = false,
            bool ignoreFields = false)
            where TContentmentListItem : class, IContentmentListItem
        {
            if (types == null)
                return Array.Empty<ConfigurationEditorModel>();

            var models = new List<ConfigurationEditorModel>();

            foreach (var type in types)
            {
                if (onlyPublic && type.IsPublic == false)
                    continue;

                var provider = Activator.CreateInstance(type) as TContentmentListItem;
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
                    Icon = provider.Icon ?? UmbIcons.DefaultIcon,
                    Fields = fields
                });
            }

            return models;
        }

        public IEnumerable<ConfigurationEditorModel> GetConfigurationEditors<TContentmentListItem>(bool onlyPublic = false, bool ignoreFields = false)
            where TContentmentListItem : class, IContentmentListItem
        {
            return GetConfigurationEditors<TContentmentListItem>(Current.TypeLoader.GetTypes<TContentmentListItem>(), onlyPublic, ignoreFields);
        }
    }
}
