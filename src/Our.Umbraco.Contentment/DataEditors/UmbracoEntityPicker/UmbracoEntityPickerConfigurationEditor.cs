/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Models.ContentEditing;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class UmbracoEntityPickerConfigurationEditor : ConfigurationEditor
    {
        private string[] _unsupportedEntityTypes;

        public UmbracoEntityPickerConfigurationEditor()
            : base()
        {
            _unsupportedEntityTypes = new[]
            {
                nameof(UmbracoEntityTypes.DictionaryItem),
                nameof(UmbracoEntityTypes.Domain),
                nameof(UmbracoEntityTypes.Language),
                nameof(UmbracoEntityTypes.MemberGroup),
                nameof(UmbracoEntityTypes.PropertyGroup),
                nameof(UmbracoEntityTypes.PropertyType),
                nameof(UmbracoEntityTypes.Stylesheet),
                nameof(UmbracoEntityTypes.User)
            };

            var items = Enum
                .GetNames(typeof(UmbracoEntityTypes))
                .OrderBy(x => x)
                .Select(x => new { value = x, name = x.SplitPascalCasing(), disabled = _unsupportedEntityTypes.Contains(x) });

            Fields.Add(
                Constants.Conventions.ConfigurationEditors.EntityType,
                "Entity Type",
                "Select the entity type to use.<br><br>Unsupported entity types have been disabled.",
                IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath),
                new Dictionary<string, object>() { { Constants.Conventions.ConfigurationEditors.Items, items } });
            Fields.AddMaxItems();
            Fields.Add(
                "allowDuplicates",
                "Allow duplicates?",
                "Select to allow the editor to select duplicate entities.",
                "boolean");
            Fields.AddHideLabel();
            Fields.AddDisableSorting();
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            config.Add("supportedTypes", new[]
            {
                nameof(UmbracoEntityTypes.DataType),
                nameof(UmbracoEntityTypes.Document),
                nameof(UmbracoEntityTypes.DocumentType),
                nameof(UmbracoEntityTypes.Media),
                nameof(UmbracoEntityTypes.MediaType),
                nameof(UmbracoEntityTypes.Member),
                nameof(UmbracoEntityTypes.MemberType)
            });

            config.Add("semisupportedTypes", new[]
            {
                nameof(UmbracoEntityTypes.Macro),
                nameof(UmbracoEntityTypes.Template)
            });

            config.Add("unsupportedTypes", _unsupportedEntityTypes);

            return config;
        }
    }
}
