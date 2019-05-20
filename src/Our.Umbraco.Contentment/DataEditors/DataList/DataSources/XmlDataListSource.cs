/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Net;
using System.Xml;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
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

        [ConfigurationField("labelXPath", "Label XPath", "textstring", Description = "Enter the XPath expression to select the label from the item (XML node select from above).")]
        public string LabelXPath { get; set; }

        public Dictionary<string, string> GetItems()
        {
            var items = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(XmlUrl) == false)
            {
                using (var client = new WebClient())
                {
                    var response = client.DownloadString(XmlUrl);
                    if (string.IsNullOrWhiteSpace(response) == false)
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(response);

                        if (string.IsNullOrWhiteSpace(ItemsXPath) == false)
                        {
                            var labelXPath = string.IsNullOrWhiteSpace(LabelXPath) == false ? LabelXPath : "text()";
                            var valueXPath = string.IsNullOrWhiteSpace(ValueXPath) == false ? ValueXPath : "text()";

                            var nodes = doc.SelectNodes(ItemsXPath);
                            foreach (XmlNode node in nodes)
                            {
                                var label = node.SelectSingleNode(labelXPath);
                                var value = node.SelectSingleNode(valueXPath);

                                if (value != null && label != null && items.ContainsKey(value.Value) == false)
                                {
                                    items.Add(value.Value, label.Value);
                                }
                            }
                        }
                    }
                }
            }

            return items;
        }
    }
}
