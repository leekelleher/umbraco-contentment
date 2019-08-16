/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class EnableMultipleConfigurationField : ConfigurationField
    {
        public const string EnableMultiple = "enableMultiple";

        public EnableMultipleConfigurationField()
            : base()
        {
            Key = EnableMultiple;
            Name = "Multiple selection?";
            Description = "Select to enable picking multiple items in the overlay.";
            View = "boolean";
        }
    }
}
