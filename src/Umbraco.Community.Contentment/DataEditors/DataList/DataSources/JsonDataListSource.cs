/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public class JsonDataListSource : IDataListSource
    {
        private readonly ILogger _logger;

        public JsonDataListSource(ILogger logger)
        {
            _logger = logger;
        }

        public string Name => "JSON Data";

        public string Description => "Configure JSON data to populate the data source.";

        public string Icon => "icon-brackets";

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "itemsJsonPath", "$..book[?(@.enabled == true)]" },
            { "nameJsonPath", "$.name" },
            { "valueJsonPath", "$.id" },
        };

        [ConfigurationField(typeof(JsonNotesConfigurationField))]
        public string Notes { get; set; }

        [ConfigurationField("url", "URL", "textstring", Description = "Enter the URL of the JSON data source.")]
        public string Url { get; set; }

        [ConfigurationField("itemsJsonPath", "Items JSONPath", "textstring", Description = "Enter the JSONPath expression to select the items from the JSON data source.")]
        public string ItemsJsonPath { get; set; }

        [ConfigurationField("nameJsonPath", "Name JSONPath", "textstring", Description = "Enter the JSONPath expression to select the name from the item.")]
        public string NameJsonPath { get; set; }

        [ConfigurationField("valueJsonPath", "Value JSONPath", "textstring", Description = "Enter the JSONPath expression to select the value (key) from the item.")]
        public string ValueJsonPath { get; set; }

        [ConfigurationField("iconXPath", "Icon JSONPath", "textstring", Description = "<em>(optional)</em> Enter the JSONPath expression to select the icon from the item.")]
        public string IconJsonPath { get; set; }

        [ConfigurationField("descriptionJsonPath", "Description JSONPath", "textstring", Description = "<em>(optional)</em> Enter the JSONPath expression to select the description from the item.")]
        public string DescriptionJsonPath { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            var items = new List<DataListItem>();

            if (string.IsNullOrWhiteSpace(Url))
                return items;

            var json = GetJson();

            if (json == null || string.IsNullOrWhiteSpace(ItemsJsonPath))
                return items;

            try
            {
                var tokens = json.SelectTokens(ItemsJsonPath);

                if (tokens.Any() == false)
                {
                    _logger.Warn<JsonDataListSource>($"The JSONPath '{ItemsJsonPath}' did not match any items in the JSON.");
                    return items;
                }

                // TODO: How would you get the string-value from a "key"?
                // This project https://github.com/s3u/JSONPath supports "~" to retrieve keys. However this is not in the original jsonpath-specs.
                // We could implement something similar, which checks the JsonPaths for a ~, and the we'll code-extract the keys. However this is a somewhat shady solution.

                foreach (var token in tokens)
                {
                    var name = token.SelectToken(NameJsonPath);
                    var value = token.SelectToken(ValueJsonPath);

                    var icon = string.IsNullOrEmpty(IconJsonPath) == false
                        ? token.SelectToken(IconJsonPath)
                        : null;

                    var description = string.IsNullOrEmpty(DescriptionJsonPath) == false
                        ? token.SelectToken(DescriptionJsonPath)
                        : null;

                    // How should we log if either name or value is empty? Note that empty or missing values are totally legal according to json
                    if (name == null)
                    {
                        _logger.Warn<JsonDataListSource>($"The JSONPath '{NameJsonPath}' did not match a 'name' in the item JSON.");
                    }

                    // If value is missing we'll skip this specific item and log as a warning
                    if (value == null)
                    {
                        _logger.Warn<JsonDataListSource>($"The JSONPath '{ValueJsonPath}' did not match a 'value' in the item XML. The item was skipped.");
                        continue;
                    }

                    items.Add(new DataListItem
                    {
                        Name = name?.ToString() ?? string.Empty,
                        Value = value?.ToString() ?? string.Empty,
                        Icon = icon?.ToString() ?? string.Empty,
                        Description = description?.ToString() ?? string.Empty
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error<JsonDataListSource>(ex, "Error finding items in the JSON. Please check the syntax of your JSONPath expressions.");
            }

            return items;
        }

        private JToken GetJson()
        {
            var content = string.Empty;

            if (Url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    using (var client = new WebClient() { Encoding = Encoding.UTF8 })
                    {
                        content = client.DownloadString(Url);
                    }
                }
                catch (WebException ex)
                {
                    _logger.Error<JsonDataListSource>(ex, $"Unable to fetch remote data from URL: {Url}");
                }
            }
            else
            {
                // assume local file
                var path = IOHelper.MapPath(Url);
                if (File.Exists(path))
                {
                    content = File.ReadAllText(path);
                }
                else
                {
                    _logger.Error<JsonDataListSource>(new FileNotFoundException(), $"Unable to find the local file path: {Url}");
                    return null;
                }
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.Warn<JsonDataListSource>($"The contents of '{Url}' was empty. Unable to process JSON data.");

                return default;
            }

            try
            {
                // Deserialize to a JToken, for general purposes.
                // Inspiration taken from StackOverflow: https://stackoverflow.com/a/38560188/12787
                return JToken.Parse(content);
            }
            catch (Exception ex)
            {
                var trimmed = content.Substring(0, Math.Min(400, content.Length));
                _logger.Error<JsonDataListSource>(ex, $"Error parsing string to JSON: {trimmed}");
            }

            return default;
        }

        class JsonNotesConfigurationField : NotesConfigurationField
        {
            public JsonNotesConfigurationField()
                : base(@"<div class=""alert alert-info"">
<p><strong>Help with your JSONPath expressions?</strong></p>
<p>This data-source uses Newtonsoft's Json.NET library, with this we are limited to extracting only the 'value' from any key/value-pairs.</p>
<p>If you need assistance with JSONPath syntax, please refer to this resource: <a href=""https://goessner.net/articles/JsonPath/"" target=""_blank"">goessner.net/articles/JsonPath</a>.</p>
</div>", true)
            { }
        }
    }
}
