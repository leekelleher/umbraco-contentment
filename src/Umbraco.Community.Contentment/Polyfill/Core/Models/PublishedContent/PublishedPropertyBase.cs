/* Copyright © 2013-present Umbraco.
 * This Source Code has been derived from Umbraco CMS.
 * https://github.com/umbraco/Umbraco-CMS/blob/release-8.18.8/src/Umbraco.Core/Models/PublishedContent/PublishedPropertyBase.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using System;
using System.Diagnostics;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Core.Models.PublishedContent
{
    [DebuggerDisplay("{Alias} ({PropertyType?.EditorAlias})")]
    internal abstract class PublishedPropertyBase : IPublishedProperty
    {
        protected PublishedPropertyBase(IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel)
        {
            PropertyType = propertyType ?? throw new ArgumentNullException(nameof(propertyType));
            ReferenceCacheLevel = referenceCacheLevel;

            ValidateCacheLevel(ReferenceCacheLevel, true);
            ValidateCacheLevel(PropertyType.CacheLevel, false);
        }

        private static void ValidateCacheLevel(PropertyCacheLevel cacheLevel, bool validateUnknown)
        {
            switch (cacheLevel)
            {
                case PropertyCacheLevel.Element:
                case PropertyCacheLevel.Elements:
                case PropertyCacheLevel.Snapshot:
                case PropertyCacheLevel.None:
                    break;
                case PropertyCacheLevel.Unknown:
                    if (!validateUnknown) goto default;
                    break;
                default:
                    throw new Exception($"Invalid cache level \"{cacheLevel}\".");
            }
        }

        public IPublishedPropertyType PropertyType { get; }

        public PropertyCacheLevel ReferenceCacheLevel { get; }

        public string Alias => PropertyType.Alias;

        public abstract bool HasValue(string culture = null, string segment = null);

        public abstract object GetSourceValue(string culture = null, string segment = null);

        public abstract object GetValue(string culture = null, string segment = null);

        public abstract object GetXPathValue(string culture = null, string segment = null);
    }
}
#endif
