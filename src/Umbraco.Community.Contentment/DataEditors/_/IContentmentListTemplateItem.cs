/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.ComponentModel;

namespace Umbraco.Community.Contentment.DataEditors
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public interface IContentmentListTemplateItem : IContentmentListItem
    {
        string NameTemplate { get; }

        string DescriptionTemplate { get; }
    }
}
