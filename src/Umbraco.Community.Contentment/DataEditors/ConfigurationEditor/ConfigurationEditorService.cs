/* Copyright © 2019 Lee Kelleher.
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
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ConfigurationEditorService
    {
        private readonly TypeLoader _typeLoader;

        public ConfigurationEditorService()
            : this(Current.TypeLoader)
        { }

        public ConfigurationEditorService(TypeLoader typeLoader)
        {
            _typeLoader = typeLoader;
        }

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

                var fields = ignoreFields == false
                    ? GetConfigurationFields(type)
                    : Enumerable.Empty<ConfigurationField>();

                models.Add(new ConfigurationEditorModel
                {
                    Type = type.GetFullNameWithAssembly(),
                    Name = provider.Name ?? type.Name.SplitPascalCasing(),
                    Description = provider.Description,
                    Icon = provider.Icon ?? Core.Constants.Icons.DefaultIcon,
                    Fields = fields,
                    DefaultValues = provider.DefaultValues,
                });
            }

            return models;
        }

        public IEnumerable<ConfigurationEditorModel> GetConfigurationEditors<TContentmentListItem>(bool onlyPublic = false, bool ignoreFields = false)
            where TContentmentListItem : class, IContentmentListItem
        {
            return GetConfigurationEditors<TContentmentListItem>(_typeLoader.GetTypes<TContentmentListItem>(), onlyPublic, ignoreFields);
        }

        internal IEnumerable<ConfigurationField> GetConfigurationFields(Type type)
        {
            var fields = new List<ConfigurationField>();

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
                        View = IOHelper.ResolveVirtualUrl(attr.View)
                    });
                }
            }

            return fields;
        }
    }
}
