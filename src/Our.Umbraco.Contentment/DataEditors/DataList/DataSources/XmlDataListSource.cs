/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Net;
using System.Xml;
using Umbraco.Core.Composing;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
#if !DEBUG
    // TODO: IsWorkInProgress - Under development.
    [HideFromTypeFinder]
#endif
    internal class XmlDataListSource : IDataListSource
    {
        public string Name => "XML";

        public string Description => "Configure the data source to use XML data.";

        public string Icon => "icon-code";

        [ConfigurationField("xmlUrl", "URL", "textstring", Description = "Enter the URL of the XML data source.")]
        public string XmlUrl { get; set; }

        [ConfigurationField("itemsXPath", "Items XPath", "textstring", Description = "Enter the XPath expression to select the items from the XML data source.")]
        public string ItemsXPath { get; set; }

        [ConfigurationField("valueXPath", "Value XPath", "textstring", Description = "Enter the XPath expression to select the value (key) from the item (XML node select from above).")]
        public string ValueXPath { get; set; }

        [ConfigurationField("nameXPath", "Name XPath", "textstring", Description = "Enter the XPath expression to select the name from the item (XML node select from above).")]
        public string NameXPath { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            var items = new List<DataListItem>();

            if (string.IsNullOrWhiteSpace(XmlUrl) == false)
            {
                try
                {
                    // TODO: [LK:2019-06-06] Maybe abstract this to a base class, share it with the JSON provider. Make it handle both local and remote paths. [LK]

                    using (var client = new WebClient())
                    {
                        var response = client.DownloadString(XmlUrl);
                        if (string.IsNullOrWhiteSpace(response) == false)
                        {
                            var doc = new XmlDocument();
                            doc.LoadXml(response);

                            if (string.IsNullOrWhiteSpace(ItemsXPath) == false)
                            {
                                var nameXPath = string.IsNullOrWhiteSpace(NameXPath) == false ? NameXPath : "text()";
                                var valueXPath = string.IsNullOrWhiteSpace(ValueXPath) == false ? ValueXPath : "text()";

                                var nodes = doc.SelectNodes(ItemsXPath);
                                foreach (XmlNode node in nodes)
                                {
                                    var name = node.SelectSingleNode(nameXPath);
                                    var value = node.SelectSingleNode(valueXPath);

                                    if (value != null && name != null)
                                    {
                                        items.Add(new DataListItem
                                        {
                                            Icon = this.Icon,
                                            Name = name.Value,
                                            Value = value.Value
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                catch (WebException)
                {
                    // TODO: [LK:2019-06-06] How to best handle this exception? [LK]
                    // Dumping it to the error log file feels careless.
                }
            }

            return items;
        }
    }
}
