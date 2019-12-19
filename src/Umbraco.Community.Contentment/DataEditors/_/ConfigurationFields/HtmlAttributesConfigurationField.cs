﻿/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class HtmlAttributesConfigurationField : ConfigurationField
    {
        internal const string HtmlAttributes = "htmlAttributes";

        public HtmlAttributesConfigurationField()
            : base()
        {
            var listFields = new[]
            {
                new ConfigurationField
                {
                    Key = "name",
                    Name = "Name",
                    View = "textstring"
                },
                new ConfigurationField
                {
                    Key = "value",
                    Name = "Value",
                    View = "textstring"
                }
            };

            Key = HtmlAttributes;
            Name = "HTML attributes";
            Description = "<em>(optional)</em> Use this field to add any HTML attributes to the list editor.";
            View = IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath);
            Config = new Dictionary<string, object>()
            {
                { DataTableConfigurationEditor.FieldItems, listFields },
                { MaxItemsConfigurationField.MaxItems, 0 },
                { DataTableConfigurationEditor.RestrictWidth, Constants.Values.True },
                { DataTableConfigurationEditor.UsePrevalueEditors, Constants.Values.True }
            };
        }
    }
}
