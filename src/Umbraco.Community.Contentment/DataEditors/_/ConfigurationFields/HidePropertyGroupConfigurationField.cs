/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class HidePropertyGroupConfigurationField : ContentmentConfigurationField
    {
        internal const string HidePropertyGroupAlias = "hidePropertyGroup";

        public HidePropertyGroupConfigurationField()
            : base()
        {
            Key = HidePropertyGroupAlias;
            Name = "Hide property group container?";
            Description = "Select to hide the editor's containing property group box. Making it appear above/outside the property group.";
            View = "boolean";
        }
    }
}
