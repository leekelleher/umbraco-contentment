/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Core.PropertyEditors;
#else
using Umbraco.Cms.Core.PropertyEditors;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class DisableSortingConfigurationField : ConfigurationField
    {
        internal const string DisableSorting = "disableSorting";

        public DisableSortingConfigurationField()
            : base()
        {
            Key = DisableSorting;
            Name = "Disable sorting?";
            Description = "Select to disable sorting of the items.";
            View = "boolean";
        }
    }
}
