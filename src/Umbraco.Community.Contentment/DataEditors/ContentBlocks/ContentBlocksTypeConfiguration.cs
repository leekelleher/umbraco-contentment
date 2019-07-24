/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ContentBlocksTypeConfiguration
    {
        [ConfigurationField(
            "elementType",
            "Element type",
            Constants.Internals.EditorsPathRoot + "readonly-node-preview.html",
            HideLabel = true)]
        public DataListItem ElementType { get; set; }

        [ConfigurationField(
            "nameTemplate",
            "Name template",
            "textstring",
            Description = "Enter an AngularJS expression to evaluate against each block for its name.")]
        public string NameTemplate { get; set; }

        [ConfigurationField(typeof(OverlaySizeConfigurationField))]
        public string OverlaySize { get; set; }

        [ConfigurationField(typeof(EnablePreviewConfigurationField))]
        public bool EnablePreview { get; set; }
    }
}
