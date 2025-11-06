/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Community.Contentment.Services
{
    public interface IContentmentContentContext
    {
        int? GetCurrentContentId(out bool isParent);

        IPublishedContent? GetCurrentContent(out bool isParent);
    }

    // NOTE: Added as a separate interface, so not to break binary backwards-compatibility. [LK]
    [Obsolete("To be combined with `IContentmentContentContext`. This interface will be removed in Contentment 8.0.")]
    public interface IContentmentContentContext2 : IContentmentContentContext
    {
        T? GetCurrentContentId<T>(out bool isParent);
    }
}
