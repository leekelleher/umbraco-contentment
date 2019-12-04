/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class XmlDataListSource : IDataListSource
    {
        private readonly ILogger _logger;
        private string _itemsXPath;
        private string _nameXPath;
        private string _valueXPath;
        private string _iconXPath;
        private string _descriptionXPath;

        public XmlDataListSource()
            : this(Current.Logger)
        { }

        public XmlDataListSource(ILogger logger)
        {
            _logger = logger;
        }

        public string Name => "XML Data";

        public string Description => "Configure XML data to populate the data source.";

        public string Icon => "icon-code";

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "itemsXPath", "//*" },
        };

        [ConfigurationField("url", "URL", "textstring", Description = "Enter the URL of the XML data source.<br>This can be either a remote URL, or local relative file path.")]
        public string Url { get; set; }

        [ConfigurationField(typeof(XmlNotesConfigurationField))]
        public string Notes { get; set; }

        [ConfigurationField("itemsXPath", "Items XPath", "textstring", Description = "Enter the XPath expression to select the items from the XML data source.")]
        public string ItemsXPath { get => _itemsXPath; set => _itemsXPath = XmlStripForXPath(value); }

        [ConfigurationField("nameXPath", "Name XPath", "textstring", Description = "Enter the XPath expression to select the name from the item.")]
        public string NameXPath { get => _nameXPath; set => _nameXPath = XmlStripForXPath(value); }

        [ConfigurationField("valueXPath", "Value XPath", "textstring", Description = "Enter the XPath expression to select the value (key) from the item.")]
        public string ValueXPath { get => _valueXPath; set => _valueXPath = XmlStripForXPath(value); }

        [ConfigurationField("iconXPath", "Icon XPath", "textstring", Description = "<em>(optional)</em> Enter the XPath expression to select the icon from the item.")]
        public string IconXPath { get => _iconXPath; set => _iconXPath = XmlStripForXPath(value); }

        [ConfigurationField("descriptionXPath", "Description XPath", "textstring", Description = "<em>(optional)</em> Enter the XPath expression to select the description from the item.")]
        public string DescriptionXPath { get => _descriptionXPath; set => _descriptionXPath = XmlStripForXPath(value); }

        public IEnumerable<DataListItem> GetItems()
        {
            var items = new List<DataListItem>();

            var doc = GetXml();

            if (doc == null || string.IsNullOrWhiteSpace(ItemsXPath))
            {
                return items;
            }

            var nodes = doc.SelectNodes(ItemsXPath);

            if (nodes.Count == 0)
            {
                _logger.Warn<string>($"Contentment | XmlDataList | Using XPath ({ ItemsXPath }) - Did not find any items in the XML: {doc.OuterXml}");
                return items;
            }

            var nameXPath = string.IsNullOrWhiteSpace(NameXPath) == false
                ? NameXPath
                : "text()";

            var valueXPath = string.IsNullOrWhiteSpace(ValueXPath) == false
                ? ValueXPath
                : "text()";

            foreach (XmlNode node in nodes)
            {
                var name = node.SelectSingleNode(NameXPath);
                var value = node.SelectSingleNode(ValueXPath);

                if (name != null && value != null)
                {
                    var icon = string.IsNullOrWhiteSpace(IconXPath) == false
                        ? node.SelectSingleNode(IconXPath)
                        : null;

                    var description = string.IsNullOrWhiteSpace(DescriptionXPath) == false
                        ? node.SelectSingleNode(DescriptionXPath)
                        : null;

                    items.Add(new DataListItem
                    {
                        Icon = icon?.Value ?? icon?.InnerText,
                        Name = name.Value ?? name.InnerText,
                        Description = description?.Value ?? description?.InnerText,
                        Value = value.Value ?? name.InnerText
                    });
                }
                else
                {
                    _logger.Warn<string>("Did not recognize a name or a value in the node XML: " + string.Concat(node.OuterXml.Take(1000)));
                    _logger.Info<string>($"Result of name XPath ({NameXPath}): " + (name != null ? name.OuterXml : "null"));
                    _logger.Info<string>($"Result of value XPath ({ValueXPath}): " + (value != null ? value.OuterXml : "null"));
                }
            }

            return items;
        }

        private XmlDocument GetXml()
        {
            if (string.IsNullOrWhiteSpace(Url))
                return null;

            var doc = new XmlDocument();

            if (Url.InvariantStartsWith("http"))
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        {
                            client.Encoding = Encoding.UTF8;
                            var xmlString = client.DownloadString(Url);
                            doc.LoadXml(XmlStripForNamespace(xmlString));

                        }
                    }
                }
                catch (WebException ex)
                {
                    _logger.Error<XmlDataListSource>(ex, "Unable to fetch remote data.");
                }
            }
            else
            {
                // assume local file
                var path = IOHelper.MapPath(Url);
                if (File.Exists(path))
                {
                    var xmlString = File.ReadAllText(path, Encoding.UTF8);
                    doc.LoadXml(XmlStripForNamespace(xmlString));
                }
                else
                {
                    _logger.Warn<XmlDataListSource>("Unable to find the local file path.");
                }
            }

            return doc;
        }

        private string XmlStripForNamespace(string xmlString)
        {
            //remove "xmlns etc." in XMLString
            xmlString = Regex.Replace(xmlString, @"xmlns(:\w+)?=""([^""]+)""", "");

            //replace ":" with "_" in node-name in XMLString
            xmlString = Regex.Replace(xmlString, @"<(\/?\w*):", "<$1_");

            //replace ":" with "_" in attributenames
            xmlString = Regex.Replace(xmlString, @":(?=\w*=[^\s])+", "_");

            return xmlString;
        }

        private string XmlStripForXPath(string xmlString)
        {
            // Replace single ":" iwith "_" in XPath, but ignores :: 
            return Regex.Replace(xmlString, @"(\/?\w*)(?<!:):(?!:)", "$1_");
        }

        class XmlNotesConfigurationField : NotesConfigurationField
        {
            public XmlNotesConfigurationField()
                : base(@"<div class=""alert alert-info"">
<p><strong>A note about your XPath expressions.</strong></p>
<p>If you need assistance with XPath syntax, please refer to this resource: <a href=""https://www.w3schools.com/xml/xpath_intro.asp"" target=""_blank""><strong>w3schools.com/xml</strong></a>.</p>
</div>", true)
            { }
        }
    }
}
