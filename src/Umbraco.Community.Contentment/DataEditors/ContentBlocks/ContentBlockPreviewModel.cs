/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Community.Contentment.DataEditors
{
    public class ContentBlockPreviewModel : ContentBlockPreviewModel<IPublishedContent, IPublishedElement>
    { }

    public class ContentBlockPreviewModel<TPublishedContent, TPublishedElement> : IContentModel
        where TPublishedContent : IPublishedContent
        where TPublishedElement : IPublishedElement
    {
        IPublishedContent IContentModel.Content => Content;

        public TPublishedContent Content { get; internal set; }

        public string ContentTypeIcon { get; set; }

        public TPublishedElement Element { get; internal set; }

        public int ElementIndex { get; set; }

        public string ElementTypeIcon { get; set; }
    }
}
