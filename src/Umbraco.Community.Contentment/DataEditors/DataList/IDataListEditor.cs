/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Umbraco.Community.Contentment.DataEditors
{
    [Obsolete("To be removed in Contentment 7.0. Migrate to use `IContentmentListEditor`.")]
    public interface IDataListEditor : IContentmentListEditor
    {
        [Obsolete("To be removed in Contentment 7.0. Migrate to use `PropertyEditorUiAlias`.")]
        string View { get; }
    }
}
