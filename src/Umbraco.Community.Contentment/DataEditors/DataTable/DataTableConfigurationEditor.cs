/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class DataTableConfigurationEditor : ConfigurationEditor
    {
        internal const string FieldItems = "fields";
        internal const string RestrictWidth = "restrictWidth";
        internal const string UsePrevalueEditors = "usePrevalueEditors";

        public DataTableConfigurationEditor(ParameterEditorCollection parameterEditors)
            : base()
        {
            // NOTE: Excluded these ParameterEditors, as they don't fully support zero-config.
            var exclusions = new[] { "contentpicker", "mediapicker", "entitypicker" };
            var paramEditors = parameterEditors
                .Select(x => new { name = x.Name, value = x.GetValueEditor().View })
                .Where(x => exclusions.Contains(x.value) == false)
                .OrderBy(x => x.name)
                .ToList();

            var listFields = new[]
            {
                new ConfigurationField
                {
                    Key = "key",
                    Name = "Key",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "label",
                    Name = "Name",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "view",
                    Name = "Editor",
                    View = IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath),
                    Config = new Dictionary<string, object>
                    {
                        { DropdownListConfigurationEditor.Items, paramEditors }
                    }
                },
            };

            Fields.Add(
                FieldItems,
                nameof(Fields),
                "Configure the editor fields for the data table.",
                IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { FieldItems, listFields },
                    { MaxItemsConfigurationField.MaxItems, 0 },
                    { DisableSortingConfigurationField.DisableSorting, Constants.Values.False },
                    { RestrictWidth, Constants.Values.True },
                    { UsePrevalueEditors, Constants.Values.False }
                });

            Fields.Add(new MaxItemsConfigurationField());
            Fields.Add(
                RestrictWidth,
                "Restrict width?",
                "Select to restrict the width of the data table. This will attempt to make the table to be the same width as the 'Add' button.",
                "boolean");
            Fields.Add(new DisableSortingConfigurationField());
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            config.Add(UsePrevalueEditors, Constants.Values.False);

            return config;
        }
    }
}
