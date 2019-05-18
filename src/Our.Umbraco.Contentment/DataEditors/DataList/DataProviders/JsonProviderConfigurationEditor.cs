/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class JsonProviderConfigurationEditor : IDataProvider
    {
        public string Name => "JSON";

        public string Description => "Configure the data source to use JSON data.";

        public string Icon => "icon-brackets";

        // TODO: Implement the XML data provider. [LK]

        public Dictionary<string, string> GetItems()
        {
            return null;
        }
    }
}
