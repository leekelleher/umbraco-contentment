/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.ComponentModel;

namespace Umbraco.Community.Contentment.DataEditors
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("To be removed in Contentment 7.0. Register a Display Mode UI on the client.")]
    public interface IContentmentDisplayMode : IContentmentEditorItem
    {
        public Dictionary<string, object>? DefaultConfig { get; }

#pragma warning disable IDE0040 // Add accessibility modifiers
        [Obsolete("To be removed in Contentment 7.0. Migrate to use `PropertyEditorUiAlias`.")]
        string View { get; }
#pragma warning restore IDE0040 // Add accessibility modifiers

        public string PropertyEditorUiAlias { get; }
    }
}
