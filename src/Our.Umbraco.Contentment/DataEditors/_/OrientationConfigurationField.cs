/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class OrientationConfigurationField : ConfigurationField
    {
        public const string Orientation = "orientation";
        public const string Horizontal = "horizontal";
        public const string Vertical = "vertical";

        public OrientationConfigurationField()
            : base()
        {
            var items = new[]
            {
                new DataListItem { Name = nameof(Horizontal), Value = Horizontal },
                new DataListItem { Name = nameof(Vertical), Value = Vertical }
            };

            Key = Orientation;
            Name = nameof(Orientation);
            Description = "Select the layout of the options.";
            View = IOHelper.ResolveUrl(RadioButtonListDataEditor.DataEditorViewPath);
            Config = new Dictionary<string, object>
            {
                { Orientation, Vertical },
                { RadioButtonListConfigurationEditor.Items, items },
                { RadioButtonListConfigurationEditor.DefaultValue, Vertical }
            };
        }
    }
}
