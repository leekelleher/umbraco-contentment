/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class PlaceholderTextConfigurationField : ConfigurationField
    {
        public const string PlaceholderText = "placeholderText";

        public PlaceholderTextConfigurationField()
        {
            Name = "Placeholder text";
            Key = PlaceholderText;
            Description = "Add placeholder text for the text input.<br>This is to be used as instructional information, not as a default value.";
            View = "textstring";
        }
    }
}
