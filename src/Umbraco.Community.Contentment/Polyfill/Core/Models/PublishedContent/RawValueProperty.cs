/* Copyright © 2013-present Umbraco.
 * This Source Code has been derived from Umbraco CMS.
 * https://github.com/umbraco/Umbraco-CMS/blob/release-8.18.8/src/Umbraco.Core/Models/PublishedContent/RawValueProperty.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using System;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Core.Models.PublishedContent
{
    internal class RawValueProperty : PublishedPropertyBase
    {
        private readonly object _sourceValue;
        private readonly Lazy<object> _objectValue;
        private readonly Lazy<object> _xpathValue;

        public RawValueProperty(IPublishedPropertyType propertyType, IPublishedElement content, object sourceValue, bool isPreviewing = false)
            : base(propertyType, PropertyCacheLevel.Unknown)
        {
            if (propertyType.Variations != ContentVariation.Nothing)
            {
                throw new ArgumentException("Property types with variations are not supported here.", nameof(propertyType));
            }

            _sourceValue = sourceValue;

            var interValue = new Lazy<object>(() => PropertyType.ConvertSourceToInter(content, _sourceValue, isPreviewing));
            _objectValue = new Lazy<object>(() => PropertyType.ConvertInterToObject(content, PropertyCacheLevel.Unknown, interValue.Value, isPreviewing));
            _xpathValue = new Lazy<object>(() => PropertyType.ConvertInterToXPath(content, PropertyCacheLevel.Unknown, interValue.Value, isPreviewing));
        }

        public override object GetSourceValue(string culture = null, string segment = null)
            => string.IsNullOrEmpty(culture) & string.IsNullOrEmpty(segment) ? _sourceValue : null;

        public override bool HasValue(string culture = null, string segment = null)
        {
            var sourceValue = GetSourceValue(culture, segment);
            return sourceValue is string s ? !string.IsNullOrWhiteSpace(s) : sourceValue != null;
        }

        public override object GetValue(string culture = null, string segment = null)
            => string.IsNullOrEmpty(culture) & string.IsNullOrEmpty(segment) ? _objectValue.Value : null;

        public override object GetXPathValue(string culture = null, string segment = null)
            => string.IsNullOrEmpty(culture) & string.IsNullOrEmpty(segment) ? _xpathValue.Value : null;
    }
}
#endif
