/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [HideFromTypeFinder]
    internal sealed class TextInputDataListEditor : IDataListEditor
    {
        public string Name => "Text Input";

        public string Description => "A textbox input, with optional values from a data-list.";

        public string Icon => TextInputDataEditor.DataEditorIcon;

        public Dictionary<string, object> DefaultConfig => default;

        public string View => IOHelper.ResolveUrl(TextInputDataEditor.DataEditorViewPath);

        [ConfigurationField("placeholderText", "Placeholder text", "textstring", Description = "Add placeholder text for the text input.<br>This is to be used as instructional information, not as a default value.")]
        public string PlaceholderText { get; set; }

        [ConfigurationField("autocomplete", "Enable autocomplete?", "boolean", Description = "Select to enable autocomplete functionality on the text input.")]
        public bool Autocomplete { get; set; }

        [ConfigurationField("maxChars", "Maximum allowed characters", "number", Description = "Enter the maximum number of characters allowed for the text input.<br>The default is a 500 character limit.")]
        public int MaxChars { get; set; }
    }
}
