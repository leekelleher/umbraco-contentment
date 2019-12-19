/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Community.Contentment.Composing;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ConfigurationEditorUtility
    {
        private readonly ContentmentListItemCollection _listItems;

        public ConfigurationEditorUtility(ContentmentListItemCollection listItems)
        {
            _listItems = listItems;
        }

        public T GetConfigurationEditor<T>(string typeName)
             where T : IContentmentListItem
        {
            if (_listItems.TryGet(typeName, out var tmp) && tmp is T item)
            {
                return item;
            }

            return default;
        }

        public IEnumerable<ConfigurationEditorModel> GetConfigurationEditors<T>(bool ignoreFields = false)
           where T : IContentmentListItem
        {
            var models = new List<ConfigurationEditorModel>();

            foreach (var item in _listItems)
            {
                if (item is T == false)
                    continue;

                var type = item.GetType();

                var fields = ignoreFields == false
                    ? GetConfigurationFields(type)
                    : Enumerable.Empty<ConfigurationField>();

                models.Add(new ConfigurationEditorModel
                {
                    Type = type.GetFullNameWithAssembly(),
                    Name = item.Name ?? type.Name.SplitPascalCasing(),
                    Description = item.Description,
                    Icon = item.Icon ?? Core.Constants.Icons.DefaultIcon,
                    Fields = fields,
                    DefaultValues = item.DefaultValues,
                });
            }

            return models;
        }

        public IEnumerable<ConfigurationField> GetConfigurationFields(Type type)
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
