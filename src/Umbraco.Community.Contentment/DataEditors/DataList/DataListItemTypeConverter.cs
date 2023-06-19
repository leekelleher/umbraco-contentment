/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Umbraco.Web;
#if NET472
using Umbraco.Core.Models.PublishedContent;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
#endif

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
                    return new DataListItem
                    {
                        Name = content.Name,
                        Description = content.TemplateId > 0 ? content.Url() : string.Empty,
                        Disabled = content.IsPublished() == false,
                        Icon = content.ContentType.GetIcon(),
                        Properties = new Dictionary<string, object>
                        {
                            { "image", content.Value<IPublishedContent>("image")?.Url() },
                        },
                        Value = content.GetUdi().ToString(),
                    };

                case JObject jobj:
                    return jobj.ToObject(typeof(DataListItem)) as DataListItem;

                case SocialLink link:
                    return new DataListItem
                    {
                        Name = link.Name,
                        Description = link.Url,
                        Icon = $"icon-{link.Network}",
                        Value = link.Network,
                    };

                default:
                    return new DataListItem
                    {
                        Name = value.ToString(),
                        Value = value.ToString(),
                    };
            }
        }
    }
}
