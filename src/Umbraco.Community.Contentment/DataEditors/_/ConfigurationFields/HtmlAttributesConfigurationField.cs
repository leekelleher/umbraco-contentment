/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class HtmlAttributesConfigurationField : ContentmentConfigurationField
    {
        internal const string HtmlAttributes = "htmlAttributes";

        public HtmlAttributesConfigurationField()
        {
            var listFields = new[]
            {
                new ContentmentConfigurationField
                {
                    Key = "name",
                    Name = "Name",
                    PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
                },
                new ContentmentConfigurationField
                {
                    Key = "value",
                    Name = "Value",
                    PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
                }
            };

            Key = HtmlAttributes;
            Name = "HTML attributes";
            Description = "<em>(optional)</em> Use this field to add any HTML attributes to the list editor.";
            PropertyEditorUiAlias = DataTableDataEditor.DataEditorUiAlias;
            Config = new Dictionary<string, object>()
            {
                { DataTableConfigurationEditor.FieldItems, listFields },
                { MaxItemsConfigurationField.MaxItems, 0 },
                { DataTableConfigurationEditor.RestrictWidth, true },
                { DataTableConfigurationEditor.UsePrevalueEditors, true }
            };
        }
    }
}
