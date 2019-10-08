/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class EnableDevModeConfigurationField : ConfigurationField
    {
        internal const string EnableDevMode = "enableDevMode";

        public EnableDevModeConfigurationField()
            : base()
        {
            Key = EnableDevMode;
            Name = "Enable developer mode?";
            Description = "Select to enable add the ability to edit the raw JSON data for the editor value.";
            View = "boolean";
        }
    }
}
