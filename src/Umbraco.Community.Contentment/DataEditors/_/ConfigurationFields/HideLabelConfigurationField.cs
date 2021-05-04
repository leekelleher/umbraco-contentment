/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class HideLabelConfigurationField : ConfigurationField
    {
        internal const string HideLabelAlias = "hideLabel";

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
