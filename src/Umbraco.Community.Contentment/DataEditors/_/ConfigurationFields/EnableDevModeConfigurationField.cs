/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class EnableDevModeConfigurationField : ContentmentConfigurationField
    {
        internal const string EnableDevMode = "enableDevMode";

        public EnableDevModeConfigurationField()
            : base()
        {
            Key = EnableDevMode;
            Name = "Developer mode?";
            Description = "Enable a property action to edit the raw data for the editor value.";
            View = "boolean";
            PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle";
        }
    }
}
