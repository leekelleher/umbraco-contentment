/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.XPath;
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
        public string ItemsXPath { get; set; }

        [ConfigurationField("nameXPath", "Name XPath", "textstring", Description = "Enter the XPath expression to select the name from the item.")]
        public string NameXPath { get; set; }

        [ConfigurationField("valueXPath", "Value XPath", "textstring", Description = "Enter the XPath expression to select the value (key) from the item.")]
        public string ValueXPath { get; set; }

        [ConfigurationField("iconXPath", "Icon XPath", "textstring", Description = "<em>(optional)</em> Enter the XPath expression to select the icon from the item.")]
        public string IconXPath { get; set; }

        [ConfigurationField("descriptionXPath", "Description XPath", "textstring", Description = "<em>(optional)</em> Enter the XPath expression to select the description from the item.")]
        public string DescriptionXPath { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            var items = new List<DataListItem>();

            var doc = GetXml();

            if (doc == null || string.IsNullOrWhiteSpace(ItemsXPath))
            {
                return items;
            }

            var ns = CreateNamespaceManager(doc);
            var nodes = doc.SelectNodes(ItemsXPath, ns);

            if (nodes.Count == 0)
            {
                _logger.Warn<string>("Did not recognize any items in the XML: " + doc.OuterXml);
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
                var name = node.SelectSingleNode(nameXPath, ns);
                var value = node.SelectSingleNode(valueXPath, ns);

                if (name != null && value != null)
                {
                    var icon = string.IsNullOrWhiteSpace(IconXPath) == false
                        ? node.SelectSingleNode(IconXPath)?.Value
                        : null;

                    var description = string.IsNullOrWhiteSpace(DescriptionXPath) == false
                        ? node.SelectSingleNode(DescriptionXPath)?.Value
                        : null;

                    items.Add(new DataListItem
                    {
                        Icon = icon,
                        Name = name.Value,
                        Description = description,
                        Value = value.Value
                    });
                }
                else
                {
                    _logger.Warn<string>("Did not recognize a name or a value in the node XML: " + node.OuterXml.Substring(0,1000));
                    _logger.Info<string>($"Result of name XPATH ({NameXPath}): " + (name != null ? name.OuterXml : "null") );
                    _logger.Info<string>($"Result of value XPATH ({ValueXPath}): " + (value != null ? value.OuterXml : "null") );
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
                            doc.LoadXml(client.DownloadString(Url));
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
                    doc.Load(path);
                }
                else
                {
                    _logger.Warn<XmlDataListSource>("Unable to find the local file path.");
                }
            }

            return doc;
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


        /// <summary>
        /// Returns an instance of XmlNamespaceManager with a specified XmlDocument.NameTable.  
        /// Returns null if no namespace is defined.
        /// </summary>
        /// <param name="doc">The XmlDocument</param>
        /// <returns>XmlNamespaceManager if there is at least one namespace, null if there is no namespace defined.</returns>
        private XmlNamespaceManager CreateNamespaceManager(XmlDocument doc)
        {
            var result = new XmlNamespaceManager(doc.NameTable);

            IDictionary<string, string> localNamespaces = null;
            XPathNavigator xNav = doc.CreateNavigator();
            while (xNav.MoveToFollowing(XPathNodeType.Element))
            {
                localNamespaces = xNav.GetNamespacesInScope(XmlNamespaceScope.Local);
                foreach (var localNamespace in localNamespaces)
                {
                    string prefix = localNamespace.Key;
                    if (string.IsNullOrEmpty(prefix))
                        prefix = "DEFAULT";

                    _logger.Debug<string>("Found and added XML namespace: " + prefix + " : " + localNamespace.Value);
                    result.AddNamespace(prefix, localNamespace.Value);
                }
            }

            return result;
        }
    }
}
