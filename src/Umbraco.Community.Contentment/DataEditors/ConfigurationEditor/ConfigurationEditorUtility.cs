/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.ComponentModel;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Strings;
using Umbraco.Community.Contentment.Composing;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ConfigurationEditorUtility
    {
        private readonly ContentmentListItemCollection _listItems;
        private readonly IShortStringHelper _shortStringHelper;

        public ConfigurationEditorUtility(
            ContentmentListItemCollection listItems,
            IShortStringHelper shortStringHelper)
        {
            _listItems = listItems;
            _shortStringHelper = shortStringHelper;
        }

        internal T? FindConfigurationEditor<T>(Func<T, bool> predicate)
            where T : IContentmentEditorItem
        {
            return _listItems.OfType<T>().FirstOrDefault(predicate);
        }

        public T? GetConfigurationEditor<T>(string key)
             where T : IContentmentEditorItem
        {
            if (string.IsNullOrWhiteSpace(key) == false && _listItems.TryGet(key, out var tmp) && tmp is T item)
            {
                return item;
            }

            return default;
        }

        public ConfigurationEditorModel? GetConfigurationEditorModel<T>(bool ignoreFields = false)
            where T : IContentmentEditorItem
        {
            return GetConfigurationEditorModel(GetConfigurationEditor<T>(typeof(T).GetFullNameWithAssembly()), ignoreFields);
        }

        public ConfigurationEditorModel? GetConfigurationEditorModel<T>(T? item, bool ignoreFields = false)
            where T : IContentmentEditorItem
        {
            if (item is null)
            {
                return default;
            }

            var type = item.GetType();

            var fields = ignoreFields == false
                ? item.Fields ?? Enumerable.Empty<ConfigurationField>()
                : Enumerable.Empty<ConfigurationField>();

            var model = new ConfigurationEditorModel
            {
                Key = type.GetFullNameWithAssembly(),
                Name = item.Name ?? type.Name.SplitPascalCasing(_shortStringHelper),
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

        public IEnumerable<ConfigurationEditorModel> GetConfigurationEditorModels<T>(bool ignoreFields = false)
            where T : IContentmentEditorItem
        {
            var models = new List<ConfigurationEditorModel>();

            foreach (var item in _listItems)
            {
                if (item is T editorItem)
                {
                    var model = GetConfigurationEditorModel(editorItem, ignoreFields);
                    if (model is not null)
                    {
                        models.Add(model);
                    }
                }
            }

            return models;
        }
    }
}
