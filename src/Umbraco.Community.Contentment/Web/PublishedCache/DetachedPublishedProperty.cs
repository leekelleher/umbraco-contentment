/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
#if NET472
using Umbraco.Core.Models.PublishedContent;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
#endif

namespace Umbraco.Community.Contentment.Web.PublishedCache
{
    internal sealed class DetachedPublishedProperty : RawValueProperty
    {
        public DetachedPublishedProperty(IPublishedPropertyType propertyType, IPublishedElement owner, object value)
            : this(propertyType, owner, value, false)
        { }

        public DetachedPublishedProperty(IPublishedPropertyType propertyType, IPublishedElement owner, object value, bool preview)
            : base(propertyType, owner, value, preview)
        { }
    }
}
