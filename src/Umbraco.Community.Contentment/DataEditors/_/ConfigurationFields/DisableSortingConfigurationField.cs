/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class DisableSortingConfigurationField : ContentmentConfigurationField
    {
        internal const string DisableSorting = "disableSorting";

        public DisableSortingConfigurationField()
            : base()
        {
            Key = DisableSorting;
            Name = "Disable sorting?";
            Description = "Select to disable sorting of the items.";
            View = "boolean";
            PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle";
        }
    }
}
