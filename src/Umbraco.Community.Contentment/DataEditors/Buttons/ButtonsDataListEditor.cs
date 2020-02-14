/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Core.Composing.HideFromTypeFinder]
    public sealed class ButtonsDataListEditor : IDataListEditor
    {
        public string Name => "Buttons";

        public string Description => "Select multiple values from a group of buttons.";

        public string Icon => "icon-tab";

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new DefaultIconConfigurationField(),
            new EnableMultipleConfigurationField()
        };

        public Dictionary<string, object> DefaultValues => default;

        public Dictionary<string, object> DefaultConfig => default;

        public string View => IOHelper.ResolveUrl(Constants.Internals.EditorsPathRoot + "buttons.html");
    }
}
