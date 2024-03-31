/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ShowDescriptionsConfigurationField : ContentmentConfigurationField
    {
        internal const string ShowDescriptions = "showDescriptions";

        public ShowDescriptionsConfigurationField()
            : base()
        {
            Key = ShowDescriptions;
            Name = "Show descriptions?";
            Description = "Enable to display the description for the item, (if available).";
            View = "views/propertyeditors/boolean/boolean.html";
            Config = new Dictionary<string, object>
            {
                { "default", Constants.Values.False }
            };
        }
    }
}
