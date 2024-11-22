/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
/* This code was originally based on code posted by Rasmus Fjord (@rasmusfjord) on the Our Umbraco forum:
 * https://our.umbraco.com/forum/umbraco-8/98381-serializing-an-publishedcontentmodel-modelsbuilder-model-in-v8#comment-310148
 *
 * From searching GitHub, and a subsequent conversation with Anders Bjerner (@abjerner), the original code came from Skybrud's internal SPA library, circa 2014.
 * The original author was René Pjengaard (@rpjengaard).
 * The Umbraco 8 packaged version of the code is available here:
 * https://github.com/skybrud/Skybrud.Umbraco.Spa/blob/v3/latest/src/Skybrud.Umbraco.Spa/Json/Resolvers/SpaPublishedContentContractResolver.cs
 * The Umbraco 9 packaged version of the code is available here:
 * https://github.com/limbo-works/Limbo.Umbraco.Spa/blob/v1/main/src/Limbo.Umbraco.Spa/Json/Resolvers/SpaPublishedContentContractResolver.cs
 *
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 */

using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

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
        private readonly Dictionary<string, Func<IPublishedContent, object>> _systemMethods;

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
                //nameof(IPublishedContent.ChildrenForAllCultures),
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
                nameof(FriendlyPublishedContentExtensions.CreatorName),
                nameof(IPublishedContent.Id),
                nameof(IPublishedContent.ItemType),
                nameof(IPublishedElement.Key),
                nameof(IPublishedContent.Level),
                nameof(IPublishedContent.Name),
                nameof(IPublishedContent.Path),
                nameof(IPublishedContent.SortOrder),
                nameof(IPublishedContent.UpdateDate),
                nameof(FriendlyPublishedContentExtensions.Url),
                nameof(IPublishedContent.UrlSegment),
                nameof(FriendlyPublishedContentExtensions.WriterName),
            };

            _systemMethods = new Dictionary<string, Func<IPublishedContent, object>>(StringComparer.OrdinalIgnoreCase)
            {
                { nameof(FriendlyPublishedContentExtensions.CreatorName), x => x.CreatorName() ?? string.Empty },
                { nameof(FriendlyPublishedContentExtensions.Url), x => x.Url() },
                { nameof(FriendlyPublishedContentExtensions.WriterName), x => x.WriterName() ?? string.Empty },
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

        public string? SystemPropertyNamePrefix { private get; set; }

        [Obsolete("Please use `SystemPropertyNamePrefix = \"_\"` instead.")]
        public bool PrefixSystemPropertyNamesWithUnderscore { private get; set; }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization).OrderBy(p => p.PropertyName, StringComparer.OrdinalIgnoreCase).ToList();

            if (typeof(IPublishedContent).IsAssignableFrom(type) == true)
            {
                var noAttributeProvider = new NoAttributeProvider();

                properties.AddRange(_systemMethods
                    .Where(x => _ignoreFromCustom.Contains(x.Key) == false)
                    .Select(x => new JsonProperty
                    {
                        DeclaringType = type,
                        PropertyName = ResolvePropertyName(x.Key),
                        UnderlyingName = x.Key,
                        PropertyType = typeof(string),
                        ValueProvider = new PublishedContentValueProvider(x.Value),
                        AttributeProvider = noAttributeProvider,
                        Readable = true,
                        Writable = false,
                        ItemIsReference = false,
                        TypeNameHandling = TypeNameHandling.None,
                    }));
            }

            return properties;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var hasSystemProperties = false;
            var property = base.CreateProperty(member, memberSerialization);

            if (typeof(IPublishedContent).IsAssignableFrom(member.DeclaringType) == true)
            {
                hasSystemProperties = true;
                property.ShouldSerialize = _ => _ignoreFromCustom.Contains(member.Name) == false && _ignoreFromContent.Contains(member.Name) == false;
            }
            else if (typeof(IPublishedElement).IsAssignableFrom(member.DeclaringType) == true)
            {
                hasSystemProperties = true;
                property.ShouldSerialize = _ => _ignoreFromCustom.Contains(member.Name) == false && _ignoreFromElement.Contains(member.Name) == false;
            }
            else if (typeof(IPublishedProperty).IsAssignableFrom(member.DeclaringType) == true)
            {
                property.ShouldSerialize = _ => _ignoreFromCustom.Contains(member.Name) == false && _ignoreFromProperty.Contains(member.Name) == false;
            }

            if (property.PropertyType is not null && _converterLookup.ContainsKey(property.PropertyType) == true)
            {
                property.Converter = _converterLookup[property.PropertyType];
            }

#pragma warning disable CS0618 // Type or member is obsolete
            if (string.IsNullOrWhiteSpace(SystemPropertyNamePrefix) == true && PrefixSystemPropertyNamesWithUnderscore == true)
            {
                SystemPropertyNamePrefix = "_";
            }
#pragma warning restore CS0618 // Type or member is obsolete

            if (hasSystemProperties == true &&
                string.IsNullOrWhiteSpace(SystemPropertyNamePrefix) == false &&
                _systemProperties.Contains(member.Name) == true)
            {
                property.PropertyName = SystemPropertyNamePrefix + property.PropertyName;
            }

            return property;
        }

        protected override string ResolvePropertyName(string propertyName)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (string.IsNullOrWhiteSpace(SystemPropertyNamePrefix) == true && PrefixSystemPropertyNamesWithUnderscore == true)
            {
                SystemPropertyNamePrefix = "_";
            }
#pragma warning restore CS0618 // Type or member is obsolete

            return string.IsNullOrWhiteSpace(SystemPropertyNamePrefix) == false && _systemProperties.Contains(propertyName) == true
                ? SystemPropertyNamePrefix + base.ResolvePropertyName(propertyName)
                : base.ResolvePropertyName(propertyName);
        }

        private class PublishedContentValueProvider : IValueProvider
        {
            private readonly Func<IPublishedContent, object> _func;

            public PublishedContentValueProvider(Func<IPublishedContent, object> func) => _func = func;

            public object GetValue(object target) => _func((IPublishedContent)target);

            public void SetValue(object target, object? value) => throw new NotImplementedException();
        }

        private class NoAttributeProvider : IAttributeProvider
        {
            public IList<Attribute> GetAttributes(bool inherit) => Array.Empty<Attribute>();

            public IList<Attribute> GetAttributes(Type attributeType, bool inherit) => Array.Empty<Attribute>();
        }
    }
}
