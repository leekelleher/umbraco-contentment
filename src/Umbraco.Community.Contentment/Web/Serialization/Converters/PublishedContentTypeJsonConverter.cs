/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Newtonsoft.Json;
#if NET472
using Umbraco.Core.Models.PublishedContent;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
#endif

namespace Umbraco.Community.Contentment.Web.Serialization
{
    public sealed class PublishedContentTypeJsonConverter : JsonConverter<IPublishedContentType>
    {
        public override IPublishedContentType ReadJson(JsonReader reader, Type objectType, IPublishedContentType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // TODO: [LK:2022-09-02] Up For Grabs! Please help me implement this.
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, IPublishedContentType value, JsonSerializer serializer)
        {
            // TODO: [LK:2022-09-02] Expand the serialized object,
            // it's mainly the `PropertyType[].ContentType` circular-reference that breaks it.
            writer.WriteValue(value?.Alias);
        }
    }
}
