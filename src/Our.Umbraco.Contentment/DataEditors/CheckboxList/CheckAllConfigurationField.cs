/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class CheckAllConfigurationField : ConfigurationField
    {
        public const string CheckAll = "checkAll";

        public CheckAllConfigurationField()
            : base()
        {
            Key = CheckAll;
            Name = "Check all?";
            Description = "Include a toggle button to select or deselect all the options?";
            View = "boolean";
        }
    }
}
