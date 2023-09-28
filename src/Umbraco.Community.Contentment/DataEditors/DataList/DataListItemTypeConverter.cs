/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class DataListItemTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => true;

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value)
            {
                case IPublishedContent content:
                    return content.ToDataListItem();

                case JObject jobj:
                    return jobj.ToObject(typeof(DataListItem)) as DataListItem;

                case SocialLink link:
                    return link.ToDataListItem();

                case string str:
                    return new DataListItem
                    {
                        Name = str,
                        Value = str
                    };

                default:
                    return new DataListItem
                    {
                        Name = value.ToString(),
                        Value = value.ToString()
                    };
            }
        }
    }
}
