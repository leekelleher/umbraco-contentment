/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class XmlProviderConfigurationEditor : IDataProvider
    {
        public string Name => "XML";

        public string Description => "Configure the data source to use XML data.";

        public string Icon => "icon-code";

        // TODO: Implement the XML data provider. [LK]

        public Dictionary<string, string> GetItems()
        {
            return null;
        }
    }
}
