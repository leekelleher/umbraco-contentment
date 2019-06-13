/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
#if !DEBUG
    // TODO: IsWorkInProgress - Under development.
    [global::Umbraco.Core.Composing.HideFromTypeFinder]
#endif
    internal class JsonDataListSource : IDataListSource
    {
        public string Name => "JSON";

        public string Description => "Configure the data source to use JSON data.";

        public string Icon => "icon-brackets";

        // TODO: [LK:2019-06-06] Implement the JSON data provider.

        [ConfigurationField("jsonUrl", "URL", "textstring", Description = "Enter the URL of the JSON data source.")]
        public string JsonUrl { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            if (string.IsNullOrWhiteSpace(JsonUrl) == false)
            {
                // Try something like... http://country.io/names.json
            }

            return null;
        }
    }
}
