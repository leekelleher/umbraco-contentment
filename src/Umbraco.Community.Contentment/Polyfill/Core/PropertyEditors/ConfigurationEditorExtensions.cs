/* This Source Code has been copied from Lee Kelleher's Umbraco Polyfill library.
 * https://github.com/leekelleher/umbraco-polyfill/blob/main/src/Core/PropertyEditors/ConfigurationEditorExtensions.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Core.Serialization;

namespace Umbraco.Core.PropertyEditors
{
    internal static class ConfigurationEditorExtensions
    {
        public static object FromDatabase(
            this IConfigurationEditor editor,
            string configurationJson,
            IConfigurationEditorJsonSerializer configurationEditorJsonSerializer)
        {
            return editor.FromDatabase(configurationJson);
        }
    }
}
#endif
