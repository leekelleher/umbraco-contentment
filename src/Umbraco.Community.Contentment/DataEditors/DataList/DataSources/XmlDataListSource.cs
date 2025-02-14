/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Net;
using System.Xml;
using System.Xml.XPath;
using Microsoft.AspNetCore.Hosting;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class XmlDataListSource : DataListToDataPickerSourceBridge, IContentmentDataSource, IContentmentListTemplateItem
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public XmlDataListSource(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public override string Name => "XML Data";

        public string? NameTemplate => default;

        public override string Description => "Configure XML data to populate the data source.";

        public string? DescriptionTemplate => "{{ url }}";

        public override string Icon => "icon-code";

        public override string Group => Constants.Conventions.DataSourceGroups.Data;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<ContentmentConfigurationField> Fields => new ContentmentConfigurationField[]
        {
            new ContentmentConfigurationField
            {
                Key = "url",
                Name = "URL",
                Description = "Enter the URL of the XML data source.<br>This can be either a remote URL, or local relative file path.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new NotesConfigurationField(@"<details class=""well well-small"">
<summary><strong>Do you need help with XPath expressions?</strong></summary>
<p>If you need assistance with XPath syntax, please refer to this resource: <a href=""https://developer.mozilla.org/en-US/docs/Web/XPath"" target=""_blank""><strong>MDN Web Docs</strong></a>.</p>
</details>
<details class=""well well-small"">
<summary><strong><em>Advanced:</em> A note about XML namespaces.</strong></summary>
<p>If your XML data source contains namespaces, these will be automatically loaded in. For default namespaces (without a prefix), these will be prefixed with ""<code>ns</code>"" followed by a number, e.g. first will be ""<code>ns1</code>"", second will be ""<code>ns2</code>"", and so forth.</p>
</details>", true),
            new ContentmentConfigurationField
            {
                Key = "itemsXPath",
                Name = "Items XPath",
                Description = "Enter the XPath expression to select the items from the XML data source.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "nameXPath",
                Name = "Name XPath",
                Description = "Enter the XPath expression to select the name from the item.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "valueXPath",
                Name = "Value XPath",
                Description = "Enter the XPath expression to select the value (key) from the item.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "iconXPath",
                Name = "Icon XPath",
                Description = "<em>(optional)</em> Enter the XPath expression to select the icon from the item.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "descriptionXPath",
                Name = "Description XPath",
                Description = "<em>(optional)</em> Enter the XPath expression to select the description from the item.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
        };

        public override Dictionary<string, object>? DefaultValues => new()
        {
            { "url", "https://leekelleher.com/umbraco/contentment/data.xml" },
            { "itemsXPath", "/items/item" },
            { "nameXPath", "@name" },
            { "valueXPath", "@value" },
            { "iconXPath", "@icon" },
            { "descriptionXPath", "@description" },
        };

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var url = config.GetValueAs("url", string.Empty);

            if (string.IsNullOrWhiteSpace(url) == true)
            {
                return Enumerable.Empty<DataListItem>();
            }

            var path = url.InvariantStartsWith("http") == false
                ? _webHostEnvironment.MapPathWebRoot(url)
                : url;

            var doc = default(XPathDocument);

            try
            {
                doc = new XPathDocument(path);
            }
            catch (HttpRequestException)
            {
                // Unable to retrieve data from '{path}'.
            }
            catch (WebException)
            {
                // Unable to retrieve data from '{path}'.
            }
            catch (XmlException)
            {
                // Unable to load XML data from '{path}'.
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
                if (string.IsNullOrWhiteSpace(prefix) == false && nsmgr.HasNamespace(prefix) == false)
                {
                    nsmgr.AddNamespace(string.IsNullOrWhiteSpace(ns.Name) == false ? ns.Name : $"ns{++idx}", ns.Value);
                }
            }

            var nodes = nav.Select(itemsXPath, nsmgr);

            if (nodes.Count == 0)
            {
                return Enumerable.Empty<DataListItem>();
            }

            var nameXPath = config.GetValueAs("nameXPath", "text()") ?? "text()";
            var valueXPath = config.GetValueAs("valueXPath", "text()") ?? "text()";
            var iconXPath = config.GetValueAs("iconXPath", string.Empty);
            var descriptionXPath = config.GetValueAs("descriptionXPath", string.Empty);

            var items = new List<DataListItem>();

            foreach (XPathNavigator node in nodes)
            {
                var value = node.SelectSingleNode(valueXPath, nsmgr);
                var name = node.SelectSingleNode(nameXPath, nsmgr) ?? value;

                if (name is not null && value is not null)
                {
                    var icon = string.IsNullOrWhiteSpace(iconXPath) == false
                        ? node.SelectSingleNode(iconXPath, nsmgr)
                        : default;

                    var description = string.IsNullOrWhiteSpace(descriptionXPath) == false
                        ? node.SelectSingleNode(descriptionXPath, nsmgr)
                        : default;

                    items.Add(new DataListItem
                    {
                        Icon = icon?.Value,
                        Name = name.Value,
                        Description = description?.Value,
                        Value = value.Value
                    });
                }
            }

            return items;
        }
    }
}
