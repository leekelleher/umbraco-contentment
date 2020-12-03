/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
/* This code has been derived from Rasmus Fjord's code, supplied on an
 * Our Umbraco forum post:
 * https://our.umbraco.com/forum/umbraco-8/98381-serializing-an-publishedcontentmodel-modelsbuilder-model-in-v8#comment-310148
 * From searching GitHub, I also found René Pjengaard's code:
 * https://github.com/rpjengaard/merchelloshop/blob/master/dev/code/Json/Resolvers/PublishedContentContractResolver.cs
 * It is unknown (to me) who the original author is, and how the code
 * has been licensed. I'll assume it's available under MIT license. */

using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Umbraco.Core.Models.PublishedContent;

namespace Umbraco.Community.Contentment.Web.Serialization
{
    public sealed class PublishedContentContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly PublishedContentContractResolver Instance = new PublishedContentContractResolver();

        private readonly Dictionary<string, JsonConverter> _converterLookup;
        private readonly HashSet<string> _ignoreFromContent;
        private readonly HashSet<string> _ignoreFromProperty;

        public PublishedContentContractResolver()
        {
            _converterLookup = new Dictionary<string, JsonConverter>(StringComparer.OrdinalIgnoreCase)
            {
                { nameof(IPublishedContent.ItemType), new StringEnumConverter() },
            };

            _ignoreFromContent = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                nameof(IPublishedContent.Children),
                nameof(IPublishedContent.ChildrenForAllCultures),
                nameof(IPublishedContent.ContentType),
                nameof(IPublishedContent.CreatorId),
                nameof(IPublishedContent.Cultures),
                nameof(IPublishedContent.Parent),
                nameof(IPublishedContent.TemplateId),
                nameof(IPublishedContent.WriterId),
            };

            _ignoreFromProperty = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                nameof(IPublishedProperty.PropertyType),
            };
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (typeof(IPublishedContent).IsAssignableFrom(member.DeclaringType))
            {
                property.ShouldSerialize = _ => _ignoreFromContent.Contains(property.PropertyName) == false;
            }
            else if (typeof(IPublishedProperty).IsAssignableFrom(member.DeclaringType))
            {
                property.ShouldSerialize = _ => _ignoreFromProperty.Contains(property.PropertyName) == false;
            }

            if (_converterLookup.ContainsKey(property.PropertyName))
            {
                property.Converter = _converterLookup[property.PropertyName];
            }

            return property;
        }
    }
}
