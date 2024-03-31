/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class EnableFilterConfigurationField : ContentmentConfigurationField
    {
        internal const string EnableFilter = "enableFilter";

        public EnableFilterConfigurationField()
            : base()
        {
            Key = EnableFilter;
            Name = "Enable filter?";
            Description = "Select to enable the search filter in the overlay selection panel.";
            View = "boolean";
        }
    }
}
