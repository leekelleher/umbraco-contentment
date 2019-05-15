/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Web.Models.ContentEditing;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class UmbracoEntityPickerModel
    {
        [JsonProperty(Constants.Conventions.ConfigurationEditors.EntityType)]
        public UmbracoEntityTypes EntityType { get; set; }

        [JsonProperty(Constants.Conventions.ConfigurationEditors.Items)]
        public GuidUdi[] Items { get; set; }
    }
}
