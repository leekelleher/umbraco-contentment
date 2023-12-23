/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class RichTextEditorDataEditor
    {
#if NET8_0_OR_GREATER
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "rich-text-editor.html";
#else
        internal const string DataEditorViewPath = "~/umbraco/views/propertyeditors/rte/rte.html";
#endif
    }
}
