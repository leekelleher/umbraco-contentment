﻿/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.IO;
using System.Net;
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
                return items;

            var nodes = doc.SelectNodes(ItemsXPath);

            var nameXPath = string.IsNullOrWhiteSpace(NameXPath) == false
                ? NameXPath
                : "text()";

            var valueXPath = string.IsNullOrWhiteSpace(ValueXPath) == false
                ? ValueXPath
                : "text()";

            foreach (XmlNode node in nodes)
            {
                var name = node.SelectSingleNode(nameXPath);
                var value = node.SelectSingleNode(valueXPath);

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
                        doc.LoadXml(client.DownloadString(Url));
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
    }
}
