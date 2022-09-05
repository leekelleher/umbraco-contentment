/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472 == false
using System;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.Community.Contentment.Web.Serialization
{
    public class HtmlEncodedStringJsonConverter : JsonConverter<IHtmlEncodedString>
    {
        public override bool CanRead => false;

        public override bool CanWrite => true;

        public override IHtmlEncodedString ReadJson(JsonReader reader, Type objectType, IHtmlEncodedString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // TODO: [LK:2022-09-02] Up For Grabs! Please help me implement this.
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, IHtmlEncodedString value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToHtmlString());
        }
    }
}
#endif
