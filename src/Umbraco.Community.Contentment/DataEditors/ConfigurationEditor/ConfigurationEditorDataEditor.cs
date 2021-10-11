/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using UmbConstants = Umbraco.Core.Constants;
#else
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ConfigurationEditorDataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "ConfigurationEditor";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Configuration Editor";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "configuration-editor.html";
        internal const string DataEditorOverlayViewPath = Constants.Internals.EditorsPathRoot + "configuration-editor.overlay.html";
        internal const string DataEditorIcon = UmbConstants.Icons.Macro;
    }
}
