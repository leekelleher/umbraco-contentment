/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    // NOTE: This configuration editor is required, so we can make use of Umbraco's `LabelValueConverter`. [LK]
    internal sealed class TemplatedLabelConfigurationEditor : ConfigurationEditor<LabelConfiguration>
    {
        public TemplatedLabelConfigurationEditor(IIOHelper ioHelper) : base(ioHelper) { }
    }
}
