/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class ReadOnlyDataValueEditor : HideLabelDataValueEditor
    {
        public ReadOnlyDataValueEditor(DataEditorAttribute attribute)
            : base(attribute)
        { }

        public override bool IsReadOnly => true;
    }
}
