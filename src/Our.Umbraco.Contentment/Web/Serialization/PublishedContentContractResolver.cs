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
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Our.Umbraco.Contentment
{
    public class PublishedContentContractResolver : DefaultContractResolver
    {
        public static readonly PublishedContentContractResolver Instance = new PublishedContentContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            property.ShouldSerialize = instance =>
            {
                if (property.PropertyName == "CompositionAliases") return false;
                if (property.PropertyName == "ContentSet") return false;
                if (property.PropertyName == "PropertyTypes") return false;
                if (property.PropertyName == "ObjectContent") return false;
                if (property.PropertyName == "Properties") return false;
                if (property.PropertyName == "Parent") return false;
                if (property.PropertyName == "Children") return false;
                if (property.PropertyName == "ChildrenForAllCultures") return false;
                if (property.PropertyName == "DocumentTypeId") return false;
                if (property.PropertyName == "WriterName") return false;
                if (property.PropertyName == "CreatorName") return false;
                if (property.PropertyName == "WriterId") return false;
                if (property.PropertyName == "CreatorId") return false;
                if (property.PropertyName == "CreateDate") return false;
                if (property.PropertyName == "UpdateDate") return false;
                if (property.PropertyName == "Version") return false;
                if (property.PropertyName == "SortOrder") return false;
                if (property.PropertyName == "TemplateId") return false;
                if (property.PropertyName == "IsDraft") return false;
                if (property.PropertyName == "ItemType") return false;
                if (property.PropertyName == "ContentType") return false;
                if (property.PropertyName == "Url") return false;
                if (property.PropertyName == "ContentSet") return false;
                //ADD CUSTOM OVERRRIDES AFTER THIS IN THE ABOVE FORMAT

                //making people on the other end of my api happy 
                //property.PropertyName = StringUtils.ToCamelCase(property.PropertyName);

                return true;
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
