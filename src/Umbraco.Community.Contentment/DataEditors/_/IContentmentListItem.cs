/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.ComponentModel;
using Umbraco.Cms.Core.Composing;

namespace Umbraco.Community.Contentment.DataEditors
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IContentmentListItem : IDiscoverable
    {
        public string? Name { get; }

        public string? Description { get; }

        public string? Icon { get; }
    }
}
