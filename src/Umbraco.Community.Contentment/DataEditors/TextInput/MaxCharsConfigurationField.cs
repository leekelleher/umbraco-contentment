/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class MaxCharsConfigurationField : ConfigurationField
    {
        public const string MaxChars = "maxChars";

        public MaxCharsConfigurationField()
        {
            Name = "Maximum allowed characters";
            Key = MaxChars;
            Description = "Enter the maximum number of characters allowed for the text input.<br>The default limit is 500 characters.";
            View = IOHelper.ResolveUrl(NumberInputDataEditor.DataEditorViewPath);
        }
    }
}
