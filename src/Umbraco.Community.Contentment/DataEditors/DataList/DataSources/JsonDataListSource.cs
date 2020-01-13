﻿/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
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
using Umbraco.Core;
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

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new NotesConfigurationField(@"<div class=""alert alert-info"">
<p><strong>Help with your JSONPath expressions?</strong></p>
<p>This data-source uses Newtonsoft's Json.NET library, with this we are limited to extracting only the 'value' from any key/value-pairs.</p>
<p>If you need assistance with JSONPath syntax, please refer to this resource: <a href=""https://goessner.net/articles/JsonPath/"" target=""_blank"">goessner.net/articles/JsonPath</a>.</p>
</div>", true),
            new ConfigurationField
            {
                Key = "url",
                Name = "URL",
                Description = "Enter the URL of the JSON data source.",
                View = "textstring",
            },
            new ConfigurationField
            {
                Key = "itemsJsonPath",
                Name = "Items JSONPath",
                Description = "Enter the JSONPath expression to select the items from the JSON data source.",
                View = "textstring",
            },
            new ConfigurationField
            {
                Key = "nameJsonPath",
                Name = "Name JSONPath",
                Description = "Enter the JSONPath expression to select the name from the item.",
                View = "textstring",
            },
            new ConfigurationField
            {
                Key = "valueJsonPath",
                Name = "Value JSONPath",
                Description = "Enter the JSONPath expression to select the value (key) from the item.",
                View = "textstring",
            },
            new ConfigurationField
            {
                Key = "iconJsonPath",
                Name = "Icon JSONPath",
                Description = "<em>(optional)</em> Enter the JSONPath expression to select the icon from the item.",
                View = "textstring",
            },
            new ConfigurationField
            {
                Key = "descriptionJsonPath",
                Name = "Description JSONPath",
                Description = "<em>(optional)</em> Enter the JSONPath expression to select the description from the item.",
                View = "textstring",
            },
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "itemsJsonPath", "$..book[?(@.enabled == true)]" },
            { "nameJsonPath", "$.name" },
            { "valueJsonPath", "$.id" },
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var items = new List<DataListItem>();

            var url = config.GetValueAs("url", string.Empty);

            if (string.IsNullOrWhiteSpace(url))
            {
                return items;
            }

            var json = GetJson(url);

            if (json == null)
            {
                return items;
            }

            var itemsJsonPath = config.GetValueAs("itemsJsonPath", string.Empty);

            if (string.IsNullOrWhiteSpace(itemsJsonPath))
            {
                return items;
            }

            try
            {
                var tokens = json.SelectTokens(itemsJsonPath);

                if (tokens.Any() == false)
                {
                    _logger.Warn<JsonDataListSource>($"The JSONPath '{itemsJsonPath}' did not match any items in the JSON.");
                    return items;
                }

                // TODO: [UP-FOR-GRABS] How would you get the string-value from a "key"?
                // This project https://github.com/s3u/JSONPath supports "~" to retrieve keys. However this is not in the original jsonpath-specs.
                // We could implement something similar, which checks the JsonPaths for a ~, and the we'll code-extract the keys. However this is a somewhat shady solution.

                var nameJsonPath = config.GetValueAs("nameJsonPath", string.Empty);
                var valueJsonPath = config.GetValueAs("valueJsonPath", string.Empty);
                var iconJsonPath = config.GetValueAs("iconJsonPath", string.Empty);
                var descriptionJsonPath = config.GetValueAs("descriptionJsonPath", string.Empty);

                foreach (var token in tokens)
                {
                    var name = token.SelectToken(nameJsonPath);
                    var value = token.SelectToken(valueJsonPath);

                    var icon = string.IsNullOrEmpty(iconJsonPath) == false
                        ? token.SelectToken(iconJsonPath)
                        : null;

                    var description = string.IsNullOrEmpty(descriptionJsonPath) == false
                        ? token.SelectToken(descriptionJsonPath)
                        : null;

                    // How should we log if either name or value is empty? Note that empty or missing values are totally legal according to json
                    if (name == null)
                    {
                        _logger.Warn<JsonDataListSource>($"The JSONPath '{nameJsonPath}' did not match a 'name' in the item JSON.");
                    }

                    // If value is missing we'll skip this specific item and log as a warning
                    if (value == null)
                    {
                        _logger.Warn<JsonDataListSource>($"The JSONPath '{valueJsonPath}' did not match a 'value' in the item XML. The item was skipped.");
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

        private JToken GetJson(string url)
        {
            var content = string.Empty;

            if (url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    using (var client = new WebClient() { Encoding = Encoding.UTF8 })
                    {
                        content = client.DownloadString(url);
                    }
                }
                catch (WebException ex)
                {
                    _logger.Error<JsonDataListSource>(ex, $"Unable to fetch remote data from URL: {url}");
                }
            }
            else
            {
                // assume local file
                var path = IOHelper.MapPath(url);
                if (File.Exists(path))
                {
                    content = File.ReadAllText(path);
                }
                else
                {
                    _logger.Error<JsonDataListSource>(new FileNotFoundException(), $"Unable to find the local file path: {url}");
                    return null;
                }
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.Warn<JsonDataListSource>($"The contents of '{url}' was empty. Unable to process JSON data.");

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
    }
}
