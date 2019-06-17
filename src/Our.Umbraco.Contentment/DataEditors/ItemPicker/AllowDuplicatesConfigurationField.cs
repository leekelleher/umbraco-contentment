/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class AllowDuplicatesConfigurationField : ConfigurationField
    {
        public const string AllowDuplicates = "allowDuplicates";

        public AllowDuplicatesConfigurationField()
            : base()
        {
            Key = AllowDuplicates;
            Name = "Allow duplicates?";
            Description = "Select to allow the editor to select duplicate items.";
            View = "boolean";
        }
    }
}
