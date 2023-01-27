/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#if NET472
using Umbraco.Core.Models.PublishedContent;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
#endif

namespace Umbraco.Community.Contentment.Web.Serialization
{
    public sealed class PublishedContentTypeJsonConverter : JsonConverter<IPublishedContentType>
    {
        public override bool CanRead => false;

        public override bool CanWrite => true;

        public override IPublishedContentType ReadJson(JsonReader reader, Type objectType, IPublishedContentType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // TODO: [UP-FOR-GRABS] Please help me implement this.
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, IPublishedContentType value, JsonSerializer serializer)
        {
#if NET472
            value.TryGetKey(out var contentTypeKey);
#endif
            JObject
                .FromObject(new
                {
                    value.Alias,
                    value.CompositionAliases,
                    value.Id,
                    value.ItemType,
                    value.IsElement,
#if NET472
                    Key = contentTypeKey,
#else
                    value.Key,
#endif
                    value.PropertyTypes,
                }, serializer)
                .WriteTo(writer);
        }
    }
}
