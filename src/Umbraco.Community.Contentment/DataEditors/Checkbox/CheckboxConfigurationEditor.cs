/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class CheckboxConfigurationEditor : ConfigurationEditor
    {
        internal const string ShowInline = "showInline";

        public CheckboxConfigurationEditor()
            : base()
        {
            Fields.Add(
                ShowInline,
                "Show label inline?",
                "Select to show the property's label and description inline next to the checkbox.<br><br>This option will make the checkbox field span the full width of the editor.",
                "boolean");
        }
    }
}
