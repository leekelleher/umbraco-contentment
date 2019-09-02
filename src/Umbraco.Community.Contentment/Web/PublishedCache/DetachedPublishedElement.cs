/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;

namespace Umbraco.Community.Contentment.Web.PublishedCache
{
    internal class DetachedPublishedElement : IPublishedElement
    {
        public DetachedPublishedElement(Guid key, IPublishedContentType contentType, IEnumerable<IPublishedProperty> properties)
        {
            Key = key;
            ContentType = contentType;
            Properties = properties;
        }

        public IPublishedContentType ContentType { get; }

        public Guid Key { get; }

        public IEnumerable<IPublishedProperty> Properties { get; }

        public IPublishedProperty GetProperty(string alias)
        {
            var index = ContentType.GetPropertyIndex(alias);

            return index > 0
                ? Properties.ElementAtOrDefault(index)
                : null;
        }
    }
}
