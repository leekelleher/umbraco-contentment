﻿/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class CheckboxListDataListEditor : IDataListEditor
    {
        public string Name => "Checkbox List";

        public string Description => "Select multiple values from a list of checkboxes.";

        public string Icon => CheckboxListDataEditor.DataEditorIcon;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new CheckAllConfigurationField(),
            new ShowDescriptionsConfigurationField(),
        };

        public Dictionary<string, object> DefaultValues => default;

        public Dictionary<string, object> DefaultConfig => default;

        public string View => IOHelper.ResolveUrl(CheckboxListDataEditor.DataEditorViewPath);
    }
}
