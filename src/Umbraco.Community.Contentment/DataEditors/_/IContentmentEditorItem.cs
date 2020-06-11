/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.ComponentModel;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IContentmentEditorItem : IContentmentListItem
    {
        Dictionary<string, object> DefaultValues { get; }

        IEnumerable<ConfigurationField> Fields { get; }

        OverlaySize OverlaySize { get; }
    }
}
