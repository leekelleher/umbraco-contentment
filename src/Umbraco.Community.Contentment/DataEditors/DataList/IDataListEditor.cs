/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Umbraco.Community.Contentment.DataEditors
{
    public interface IDataListEditor : IContentmentEditorItem
    {
        Dictionary<string, object>? DefaultConfig { get; }

        bool HasMultipleValues(Dictionary<string, object>? config);

        [Obsolete("Migrate to use `PropertyEditorUiAlias`.")]
        string View { get; }

        string PropertyEditorUiAlias { get; }
    }
}
