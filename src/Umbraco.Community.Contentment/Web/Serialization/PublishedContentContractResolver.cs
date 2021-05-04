/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
/* This code was originally based on code by Rasmus Fjord,
 * supplied on an Our Umbraco forum post:
 * https://our.umbraco.com/forum/umbraco-8/98381-serializing-an-publishedcontentmodel-modelsbuilder-model-in-v8#comment-310148
 * From searching GitHub, I also found René Pjengaard's code:
 * https://github.com/rpjengaard/merchelloshop/blob/master/dev/code/Json/Resolvers/PublishedContentContractResolver.cs
 * It is unknown (to me) who the original author is, and how the code
 * has been licensed. My assumption is under the MIT license. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Community.Contentment.Web.Serialization
{
    public sealed class PublishedContentContractResolver : DefaultContractResolver
    {
        public static readonly PublishedContentContractResolver Instance = new PublishedContentContractResolver();

        private readonly Dictionary<Type, JsonConverter> _converterLookup;
        private readonly HashSet<string> _ignoreFromCustom;
        private readonly HashSet<string> _ignoreFromElement;
        private readonly HashSet<string> _ignoreFromContent;
        private readonly HashSet<string> _ignoreFromProperty;
        private readonly HashSet<string> _systemProperties;

        public PublishedContentContractResolver()
            : base()
        {
            NamingStrategy = new CamelCaseNamingStrategy
            {
                OverrideSpecifiedNames = true,
                ProcessDictionaryKeys = true,
            };

            _converterLookup = new Dictionary<Type, JsonConverter>()
            {
                { typeof(PublishedItemType), new StringEnumConverter() }
            };

            _ignoreFromCustom = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            _ignoreFromElement = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                nameof(IPublishedElement.ContentType),
                nameof(IPublishedElement.Properties),
            };

            _ignoreFromContent = new HashSet<string>(_ignoreFromElement, StringComparer.OrdinalIgnoreCase)
            {
                nameof(IPublishedContent.Children),
                nameof(IPublishedContent.ChildrenForAllCultures),
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

            _systemProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                nameof(IPublishedContent.CreateDate),
#pragma warning disable CS0618 // Type or member is obsolete
                nameof(IPublishedContent.CreatorName),
#pragma warning restore CS0618 // Type or member is obsolete
                nameof(IPublishedContent.Id),
                nameof(IPublishedContent.ItemType),
                nameof(IPublishedElement.Key),
                nameof(IPublishedContent.Level),
                nameof(IPublishedContent.Name),
                nameof(IPublishedContent.Path),
                nameof(IPublishedContent.SortOrder),
                nameof(IPublishedContent.UpdateDate),
#pragma warning disable CS0618 // Type or member is obsolete
                nameof(IPublishedContent.Url),
#pragma warning restore CS0618 // Type or member is obsolete
                nameof(IPublishedContent.UrlSegment),
#pragma warning disable CS0618 // Type or member is obsolete
                nameof(IPublishedContent.WriterName),
#pragma warning restore CS0618 // Type or member is obsolete
            };
        }

        public string[] PropertiesToIgnore
        {
            set => _ignoreFromCustom.UnionWith(value);
        }

        public Dictionary<Type, JsonConverter> PropertyTypeConverters
        {
            set
            {
                foreach (var item in value)
                {
                    if (_converterLookup.ContainsKey(item.Key) == false)
                    {
                        _converterLookup.Add(item.Key, item.Value);
                    }
                }
            }
        }

        public bool PrefixSystemPropertyNamesWithUnderscore { private get; set; }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization).OrderBy(p => p.PropertyName).ToList();
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (_ignoreFromCustom.Contains(member.Name) == true)
            {
                property.ShouldSerialize = _ => false;
            }
            else if (typeof(IPublishedContent).IsAssignableFrom(member.DeclaringType) == true)
            {
                property.ShouldSerialize = _ => _ignoreFromContent.Contains(member.Name) == false;
            }
            else if (typeof(IPublishedElement).IsAssignableFrom(member.DeclaringType) == true)
            {
                property.ShouldSerialize = _ => _ignoreFromElement.Contains(member.Name) == false;
            }
            else if (typeof(IPublishedProperty).IsAssignableFrom(member.DeclaringType) == true)
            {
                property.ShouldSerialize = _ => _ignoreFromProperty.Contains(member.Name) == false;
            }

            if (_converterLookup.ContainsKey(property.PropertyType) == true)
            {
                property.Converter = _converterLookup[property.PropertyType];
            }

            if (PrefixSystemPropertyNamesWithUnderscore == true && _systemProperties.Contains(member.Name) == true)
            {
                property.PropertyName = "_" + property.PropertyName;
            }

            return property;
        }
    }
}
