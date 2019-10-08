/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class CheckAllConfigurationField : ConfigurationField
    {
        internal const string CheckAll = "checkAll";

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
