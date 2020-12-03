/* Copyright © 2016 UMCO, Our Umbraco and other contributors.
 * This Source Code has been derived from Inner Content.
 * https://github.com/umco/umbraco-inner-content/blob/2.0.4/src/Our.Umbraco.InnerContent/Models/DetachedPublishedProperty.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.Web.PublishedCache
{
    internal sealed class DetachedPublishedProperty : IPublishedProperty
    {
        private readonly object _sourceValue;
        private readonly Lazy<object> _interValue;
        private readonly Lazy<object> _objectValue;
        private readonly Lazy<object> _xpathValue;

        public DetachedPublishedProperty(IPublishedPropertyType propertyType, IPublishedElement owner, object value)
            : this(propertyType, owner, value, false)
        { }

        public DetachedPublishedProperty(IPublishedPropertyType propertyType, IPublishedElement owner, object value, bool preview)
        {
            PropertyType = propertyType;

            _sourceValue = value;

            _interValue = new Lazy<object>(() => PropertyType.ConvertSourceToInter(owner, _sourceValue, preview));
            _objectValue = new Lazy<object>(() => PropertyType.ConvertInterToObject(owner, PropertyCacheLevel.Unknown, _interValue.Value, preview));
            _xpathValue = new Lazy<object>(() => PropertyType.ConvertInterToXPath(owner, PropertyCacheLevel.Unknown, _interValue.Value, preview));
        }

        public IPublishedPropertyType PropertyType { get; }

        public string Alias => PropertyType.Alias;

        public object GetSourceValue(string culture = null, string segment = null) => _sourceValue;

        public object GetValue(string culture = null, string segment = null) => _objectValue.Value;

        public object GetXPathValue(string culture = null, string segment = null) => _xpathValue.Value;

        public bool HasValue(string culture = null, string segment = null) => _sourceValue != null && _sourceValue.ToString().Trim().Length > 0;
    }
}
