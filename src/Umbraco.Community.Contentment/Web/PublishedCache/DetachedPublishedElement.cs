/* Copyright © 2016 UMCO, Our Umbraco and other contributors.
 * This Source Code has been derived from Inner Content.
 * https://github.com/umco/umbraco-inner-content/blob/2.0.4/src/Our.Umbraco.InnerContent/Models/DetachedPublishedContent.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Community.Contentment.Web.PublishedCache
{
    internal sealed class DetachedPublishedElement : IPublishedElement
    {
        private readonly Dictionary<string, IPublishedProperty> _propertyLookup;

        public DetachedPublishedElement(Guid key, IPublishedContentType contentType, IEnumerable<IPublishedProperty> properties)
        {
            Key = key;
            Name = key.ToString();
            ContentType = contentType;
            Cultures = new Dictionary<string, PublishedCultureInfo>();
            Properties = properties;

            _propertyLookup = properties.ToDictionary(x => x.Alias, StringComparer.OrdinalIgnoreCase);
        }

        public IPublishedContentType ContentType { get; }

        public Guid Key { get; }

        public IEnumerable<IPublishedProperty> Properties { get; }

        public int Id { get; }

        public string Name { get; }

        public int SortOrder { get; }

        public int CreatorId { get; }

        public DateTime CreateDate { get; }

        public int WriterId { get; }

        public DateTime UpdateDate { get; }

        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures { get; }

        public PublishedItemType ItemType => PublishedItemType.Element;

        public IPublishedProperty? GetProperty(string alias)
        {
            return _propertyLookup.ContainsKey(alias)
                ? _propertyLookup[alias]
                : default;
        }

        public bool IsDraft(string? culture = null) => false;

        public bool IsPublished(string? culture = null) => true;
    }
}
