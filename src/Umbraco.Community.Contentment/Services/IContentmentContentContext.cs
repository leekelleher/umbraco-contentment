/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Core.Models.PublishedContent;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
#endif

namespace Umbraco.Community.Contentment.Services
{
    public interface IContentmentContentContext
    {
        int? GetCurrentContentId();

        int? GetCurrentContentId(out bool isParent);

        IPublishedContent GetCurrentContent();

        IPublishedContent GetCurrentContent(out bool isParent);
    }
}
