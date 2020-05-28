/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Core.Composing.HideFromTypeFinder]
    public sealed class TextInputDataListEditor : IDataListEditor
    {
        public string Name => "Text Input";

        public string Description => "A textbox input, with optional values from a data-list.";

        public string Icon => TextInputDataEditor.DataEditorIcon;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new PlaceholderTextConfigurationField(),
            new AutocompleteConfigurationField(),
            new MaxCharsConfigurationField(),
        };

        public Dictionary<string, object> DefaultConfig => default;

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { MaxCharsConfigurationField.MaxChars, 500 }
        };

        public bool HasMultipleValues(Dictionary<string, object> config) => false;

        public string View => TextInputDataEditor.DataEditorViewPath;
    }
}
