/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Umbraco.Community.Contentment.DataEditors
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DimensionsModel
    {
        public int Height { get; set; }

        public int Width { get; set; }

        public string ToCssDeclaration()
        {
            return $"width:{Width}px;height:{Height}px;";
        }

        public IHtmlString ToHtmlAttributes()
        {
            return new HtmlString($"width=\"{Width}\" height=\"{Height}\"");
        }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}
