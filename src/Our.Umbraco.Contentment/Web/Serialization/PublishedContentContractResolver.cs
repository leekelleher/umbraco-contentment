/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
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
using Newtonsoft.Json.Serialization;

namespace Our.Umbraco.Contentment
{
    public class PublishedContentContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly PublishedContentContractResolver Instance = new PublishedContentContractResolver();

        private readonly HashSet<string> _excludedProperties;

        public PublishedContentContractResolver()
        {
            _excludedProperties = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
            {
                "Children",
                "ChildrenForAllCultures",
                "CompositionAliases",
                "ContentSet",
                "ContentType",
                "CreateDate",
                "CreatorId",
                "CreatorName",
                "DocumentTypeId",
                "IsDraft",
                "ItemType",
                "ObjectContent",
                "Parent",
                "Properties",
                "PropertyTypes",
                "SortOrder",
                "TemplateId",
                "UpdateDate",
                "Url",
                "Version",
                "WriterId",
                "WriterName",
            };
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);


            property.ShouldSerialize = _ =>
            {
                return _excludedProperties.Contains(property.PropertyName) == false;
            };

            return property;
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            return contract;
        }
    }
}
