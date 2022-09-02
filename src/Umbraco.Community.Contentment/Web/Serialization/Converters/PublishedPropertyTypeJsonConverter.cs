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
    public sealed class PublishedPropertyTypeJsonConverter : JsonConverter<IPublishedPropertyType>
    {
        public override IPublishedPropertyType ReadJson(JsonReader reader, Type objectType, IPublishedPropertyType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // TODO: [LK:2022-09-05] Up For Grabs! Please help me implement this.
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, IPublishedPropertyType value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.Alias);
        }
    }
}
