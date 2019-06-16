/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class HideLabelConfigurationField : ConfigurationField
    {
        public const string HideLabelAlias = "hideLabel";

        public HideLabelConfigurationField()
            : base()
        {
            Key = HideLabelAlias;
            Name = "Hide label?";
            Description = "Select to hide the label and have the editor take up the full width of the panel.";
            View = "boolean";
        }
    }
}
