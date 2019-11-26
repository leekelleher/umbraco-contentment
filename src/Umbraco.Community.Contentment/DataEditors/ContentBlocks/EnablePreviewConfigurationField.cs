/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class EnablePreviewConfigurationField : ConfigurationField
    {
        public EnablePreviewConfigurationField()
        {
            Key = "enablePreview";
            Name = "Enable preview?";
            Description = "Select to enable a HTML preview for this content block type.";
            View = IOHelper.ResolveUrl(CheckboxDataEditor.DataEditorViewPath);
            Config = new Dictionary<string, object>
            {
                { CheckboxConfigurationEditor.ShowInline, Constants.Values.True },
            };
            HideLabel = true;
        }
    }
}
