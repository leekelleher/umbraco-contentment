/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class XmlDataListSource : IDataListSource
    {
        private readonly ILogger<XmlDataListSource> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IIOHelper _ioHelper;

        public XmlDataListSource(
            ILogger<XmlDataListSource> logger,
            IHostingEnvironment hostingEnvironment,
            IIOHelper ioHelper)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _ioHelper = ioHelper;
        }

        public string Name => "XML Data";

        public string Description => "Configure XML data to populate the data source.";

        public string Icon => "icon-code";

        public string Group => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ConfigurationField
            {
                Key = "url",
                Name = "URL",
                Description = "Enter the URL of the XML data source.<br>This can be either a remote URL, or local relative file path.",
                View = "textstring"
            },
            new NotesConfigurationField(_ioHelper, @"<details class=""well well-small"">
<summary><strong>Do you need help with XPath expressions?</strong></summary>
<p>If you need assistance with XPath syntax, please refer to this resource: <a href=""https://www.w3schools.com/xml/xpath_intro.asp"" target=""_blank""><strong>w3schools.com/xml</strong></a>.</p>
</details>
<details class=""well well-small"">
<summary><strong><em>Advanced:</em> A note about XML namespaces.</strong></summary>
<p>If your XML data source contains namespaces, these will be automatically loaded in. For default namespaces (without a prefix), these will be prefixed with ""<code>ns</code>"" followed by a number, e.g. first will be ""<code>ns1</code>"", second will be ""<code>ns2</code>"", and so forth.</p>
</details>", true),
            new ConfigurationField
            {
                Key = "itemsXPath",
                Name = "Items XPath",
                Description = "Enter the XPath expression to select the items from the XML data source.",
                View =  "textstring",
            },
            new ConfigurationField
            {
                Key = "nameXPath",
                Name = "Name XPath",
                Description = "Enter the XPath expression to select the name from the item.",
                View =  "textstring",
            },
            new ConfigurationField
            {
                Key = "valueXPath",
                Name = "Value XPath",
                Description = "Enter the XPath expression to select the value (key) from the item.",
                View =  "textstring",
            },
            new ConfigurationField
            {
                Key = "iconXPath",
                Name = "Icon XPath",
                Description = "<em>(optional)</em> Enter the XPath expression to select the icon from the item.",
                View = "textstring",
            },
            new ConfigurationField
            {
                Key = "descriptionXPath",
                Name = "Description XPath",
                Description = "<em>(optional)</em> Enter the XPath expression to select the description from the item.",
                View = "textstring",
            },
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "itemsXPath", "//*" },
            { "nameXPath", "text()" },
            { "valueXPath", "text()" },
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var url = config.GetValueAs("url", string.Empty);

            if (string.IsNullOrWhiteSpace(url) == true)
            {
                return Enumerable.Empty<DataListItem>();
            }

            var path = url.InvariantStartsWith("http") == false
                ? _hostingEnvironment.MapPathWebRoot(url)
                : url;

            var doc = default(XPathDocument);

            try
            {
                // TODO: [LK:2021-05-07] Doesn't work for HTTPS URLs, throws a `HttpRequestException`.
                doc = new XPathDocument(path);
            }
            catch(HttpRequestException ex)
            {
                _logger.LogError(ex, $"Unable to retrieve data from '{path}'.");
            }
            catch (WebException ex)
            {
                _logger.LogError(ex, $"Unable to retrieve data from '{path}'.");
            }
            catch (XmlException ex)
            {
                _logger.LogError(ex, $"Unable to load XML data from '{path}'.");
            }

            if (doc == null)
            {
                return Enumerable.Empty<DataListItem>();
            }

            var itemsXPath = config.GetValueAs("itemsXPath", string.Empty);

            if (string.IsNullOrWhiteSpace(itemsXPath) == true)
            {
                return Enumerable.Empty<DataListItem>();
            }

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

            var nodes = nav.Select(itemsXPath, nsmgr);

            if (nodes.Count == 0)
            {
                _logger.LogWarning($"The XPath '{itemsXPath}' did not match any items in the XML: {nav.OuterXml.Substring(0, Math.Min(300, nav.OuterXml.Length))}");
                return Enumerable.Empty<DataListItem>();
            }

            var nameXPath = config.GetValueAs("nameXPath", "text()");
            var valueXPath = config.GetValueAs("valueXPath", "text()");
            var iconXPath = config.GetValueAs("iconXPath", string.Empty);
            var descriptionXPath = config.GetValueAs("descriptionXPath", string.Empty);

            var items = new List<DataListItem>();

            foreach (XPathNavigator node in nodes)
            {
                var name = node.SelectSingleNode(nameXPath, nsmgr);
                var value = node.SelectSingleNode(valueXPath, nsmgr);

                if (name != null && value != null)
                {
                    var icon = string.IsNullOrWhiteSpace(iconXPath) == false
                        ? node.SelectSingleNode(iconXPath, nsmgr)
                        : null;

                    var description = string.IsNullOrWhiteSpace(descriptionXPath) == false
                        ? node.SelectSingleNode(descriptionXPath, nsmgr)
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
                    var outerXml = node.OuterXml.Substring(0, Math.Min(300, node.OuterXml.Length));

                    if (name == null)
                    {
                        _logger.LogWarning($"The XPath '{nameXPath}' did not match a 'name' in the item XML: {outerXml}");
                    }

                    if (value == null)
                    {
                        _logger.LogWarning($"The XPath '{valueXPath}' did not match a 'value' in the item XML: {outerXml}");
                    }
                }
            }

            return items;
        }
    }
}
