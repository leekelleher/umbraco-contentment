/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Umbraco.Community.Contentment.Composing;
#if NET472
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Strings;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ConfigurationEditorUtility
    {
        private readonly ContentmentListItemCollection _listItems;

        public ConfigurationEditorUtility(ContentmentListItemCollection listItems)
        {
            _listItems = listItems;
        }

        internal T FindConfigurationEditor<T>(Func<T, bool> predicate)
            where T : IContentmentEditorItem
        {
            return _listItems.OfType<T>().FirstOrDefault(predicate);
        }

        public T GetConfigurationEditor<T>(string key)
             where T : IContentmentEditorItem
        {
            if (string.IsNullOrWhiteSpace(key) == false && _listItems.TryGet(key, out var tmp) && tmp is T item)
            {
                return item;
            }

            return default;
        }

        public ConfigurationEditorModel GetConfigurationEditorModel<T>(IShortStringHelper shortStringHelper, bool ignoreFields = false)
            where T : IContentmentEditorItem
        {
            return GetConfigurationEditorModel(GetConfigurationEditor<T>(typeof(T).GetFullNameWithAssembly()), shortStringHelper, ignoreFields);
        }

        public ConfigurationEditorModel GetConfigurationEditorModel<T>(T item, IShortStringHelper shortStringHelper, bool ignoreFields = false)
            where T : IContentmentEditorItem
        {
            var type = item.GetType();

            var fields = ignoreFields == false
                ? item.Fields
                : Enumerable.Empty<ConfigurationField>();

            var model = new ConfigurationEditorModel
            {
                Key = type.GetFullNameWithAssembly(),
                Name = item.Name ?? type.Name.SplitPascalCasing(shortStringHelper),
                Description = item.Description,
                Icon = item.Icon ?? UmbConstants.Icons.DefaultIcon,
                Group = item.Group,
                Fields = fields,
                DefaultValues = item.DefaultValues,
                OverlaySize = item.OverlaySize,
            };

            if (item is IContentmentListTemplateItem lti)
            {
                model.Expressions = new Dictionary<string, string>();

                if (string.IsNullOrWhiteSpace(lti.NameTemplate) == false)
                {
                    model.Expressions.Add("name", lti.NameTemplate);
                }

                if (string.IsNullOrWhiteSpace(lti.DescriptionTemplate) == false)
                {
                    model.Expressions.Add("description", lti.DescriptionTemplate);
                }
            }

            return model;
        }

        public IEnumerable<ConfigurationEditorModel> GetConfigurationEditorModels<T>(IShortStringHelper shortStringHelper, bool ignoreFields = false)
            where T : IContentmentEditorItem
        {
            var models = new List<ConfigurationEditorModel>();

            foreach (var item in _listItems)
            {
                if (item is T editorItem)
                {
                    models.Add(GetConfigurationEditorModel(editorItem, shortStringHelper, ignoreFields));
                }
            }

            return models;
        }
    }
}
