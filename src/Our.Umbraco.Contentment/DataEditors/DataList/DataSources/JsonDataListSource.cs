/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class JsonDataListSource : IDataListSource
    {
        public string Name => "JSON";

        public string Description => "Configure the data source to use JSON data.";

        public string Icon => "icon-brackets";

        // TODO: Implement the JSON data provider. [LK]

        [ConfigurationField("jsonUrl", "URL", "textstring", Description = "Enter the URL of the JSON data source.")]
        public string JsonUrl { get; set; }

        public Dictionary<string, string> GetItems()
        {
            if (string.IsNullOrWhiteSpace(JsonUrl) == false)
            {
                // TODO: Try something like... http://country.io/names.json
            }

            return null;
        }
    }
}
