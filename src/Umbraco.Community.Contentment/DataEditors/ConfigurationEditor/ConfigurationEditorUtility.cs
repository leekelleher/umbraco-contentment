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

        public ConfigurationEditorModel GetConfigurationEditorModel<T>(bool ignoreFields = false)
            where T : IContentmentListItem
        {
            return GetConfigurationEditorModel(GetConfigurationEditor<T>(typeof(T).GetFullNameWithAssembly()), ignoreFields);
        }

        public ConfigurationEditorModel GetConfigurationEditorModel<T>(T item, bool ignoreFields = false)
            where T : IContentmentListItem
        {
            var type = item.GetType();

            var fields = ignoreFields == false
                ? item.Fields
                : Enumerable.Empty<ConfigurationField>();

            return new ConfigurationEditorModel
            {
                Type = type.GetFullNameWithAssembly(),
                Name = item.Name ?? type.Name.SplitPascalCasing(),
                Description = item.Description,
                Icon = item.Icon ?? Core.Constants.Icons.DefaultIcon,
                Fields = fields,
                DefaultValues = item.DefaultValues,
            };
        }

        public IEnumerable<ConfigurationEditorModel> GetConfigurationEditorModels<T>(bool ignoreFields = false)
           where T : IContentmentListItem
        {
            var models = new List<ConfigurationEditorModel>();

            foreach (var item in _listItems)
            {
                if (item is T == false)
                    continue;

                models.Add(GetConfigurationEditorModel(item, ignoreFields));
            }

            return models;
        }
    }
}
