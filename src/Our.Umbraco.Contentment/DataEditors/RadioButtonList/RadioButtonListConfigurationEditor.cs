/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class RadioButtonListConfigurationEditor : ConfigurationEditor
    {
        public const string Items = Constants.Conventions.ConfigurationEditors.Items;
        public const string DefaultValue = Constants.Conventions.ConfigurationEditors.DefaultValue;
        public const string HideLabel = Constants.Conventions.ConfigurationEditors.HideLabel;
        public const string Orientation = "orientation";

        public RadioButtonListConfigurationEditor()
            : base()
        {
            var listFields = new[]
            {
                new ConfigurationField
                {
                    Key = "name",
                    Name = "Name",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "value",
                    Name = "Value",
                    View = "textbox"
                }
            };

            Fields.Add(
                Items,
                "Options",
                "Configure the option items for the radiobutton list.",
                IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { DataTableConfigurationEditor.FieldItems, listFields },
                    { DataTableConfigurationEditor.MaxItems, 0 },
                    { DataTableConfigurationEditor.DisableSorting, Constants.Values.False },
                    { DataTableConfigurationEditor.RestrictWidth, Constants.Values.True },
                    { DataTableConfigurationEditor.UsePrevalueEditors, Constants.Values.False }
                });

            Fields.Add(new OrientationConfigurationField());

            Fields.AddHideLabel();
        }

        internal class OrientationConfigurationField : ConfigurationField
        {
            public const string Horizontal = "horizontal";
            public const string Vertical = "vertical";

            public OrientationConfigurationField()
                : base()
            {
                var items = new[]
                {
                    new { name = nameof(Horizontal), value = Horizontal },
                    new { name = nameof(Vertical), value = Vertical }
                };

                Key = Orientation;
                Name = nameof(Orientation);
                Description = "Select the layout of the options.";
                View = IOHelper.ResolveUrl(RadioButtonListDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    { Orientation, Horizontal },
                    { Items, items },
                    { DefaultValue, Vertical }
                };
            }
        }
    }
}
