﻿/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class XmlDataListSource : IDataListSource
    {
        private readonly ILogger _logger;

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
            { "nameXPath", "text()" },
            { "valueXPath", "text()" },
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

        [ConfigurationField(typeof(XmlNotesConfigurationField2))]
        public string Notes2 { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            var items = new List<DataListItem>();

            if (string.IsNullOrWhiteSpace(Url))
                return items;

            var path = Url.InvariantStartsWith("http") == false
                ? IOHelper.MapPath(Url)
                : Url;

            var doc = default(XPathDocument);

            try
            {
                doc = new XPathDocument(path);
            }
            catch (XmlException ex)
            {
                _logger.Error<XmlDataListSource>(ex, "Unable to load XML data.");
            }

            if (doc == null || string.IsNullOrWhiteSpace(ItemsXPath))
                return items;

            var nav = doc.CreateNavigator();

            var namespaces = nav.Select("//namespace::*");
            var nsmgr = new XmlNamespaceManager(nav.NameTable);

            var idx = 0;
            foreach (XPathNavigator ns in namespaces)
            {
                var prefix = nsmgr.LookupPrefix(ns.Value);
                if (nsmgr.HasNamespace(prefix) == false)
                {
                    nsmgr.AddNamespace(string.IsNullOrWhiteSpace(ns.Name) == false ? ns.Name : $"ns{++idx}", ns.Value);
                }
            }

            var nodes = nav.Select(ItemsXPath, nsmgr);

            if (nodes.Count == 0)
            {
                _logger.Warn<string>($"Contentment | XmlDataList | Using XPath ({ ItemsXPath }) - Did not find any items in the XML: {nav.OuterXml.Substring(0, Math.Min(300, nav.OuterXml.Length))}");
                return items;
            }

            var nameXPath = string.IsNullOrWhiteSpace(NameXPath) == false
                ? NameXPath
                : "text()";

            var valueXPath = string.IsNullOrWhiteSpace(ValueXPath) == false
                ? ValueXPath
                : "text()";

            foreach (XPathNavigator node in nodes)
            {
                var name = node.SelectSingleNode(nameXPath, nsmgr);
                var value = node.SelectSingleNode(valueXPath, nsmgr);

                if (name != null && value != null)
                {
                    var icon = string.IsNullOrWhiteSpace(IconXPath) == false
                        ? node.SelectSingleNode(IconXPath, nsmgr)
                        : null;

                    var description = string.IsNullOrWhiteSpace(DescriptionXPath) == false
                        ? node.SelectSingleNode(DescriptionXPath, nsmgr)
                        : null;

                    items.Add(new DataListItem
                    {
                        Icon = icon?.Value,
                        Name = name.Value,
                        Description = description?.Value,
                        Value = value.Value
                    });
                }
                else
                {
                    _logger.Warn<string>("Did not recognize a name or a value in the node XML: " + node.OuterXml.Substring(0, Math.Min(300, node.OuterXml.Length)));
                    _logger.Info<string>($"Result of name XPath ({NameXPath}): " + (name != null ? name.OuterXml : "null"));
                    _logger.Info<string>($"Result of value XPath ({ValueXPath}): " + (value != null ? value.OuterXml : "null"));
                }
            }

            return items;
        }

        class XmlNotesConfigurationField : NotesConfigurationField
        {
            public XmlNotesConfigurationField()
                : base(@"<div class=""alert alert-info"">
<p><strong>Help with your XPath expressions?</strong></p>
<p>If you need assistance with XPath syntax, please refer to this resource: <a href=""https://www.w3schools.com/xml/xpath_intro.asp"" target=""_blank""><strong>w3schools.com/xml</strong></a>.</p>
</div>", true)
            { }
        }

        class XmlNotesConfigurationField2 : NotesConfigurationField
        {
            public XmlNotesConfigurationField2()
                : base(@"<div class=""alert alert-warning"">
<p><strong><em>Advanced:</em> A note about XML namespaces.</strong></p>
<p>If your XML data source contains namespaces, these will be automatically loaded in. For default namespaces (without a prefix), these will be prefixed with ""<code>ns</code>"" followed by a number, e.g. first will be ""<code>ns1</code>"", second will be ""<code>ns2</code>"", and so forth.</p>
</div>", true)
            {
                Key = "notes2";
            }
        }
    }
}
