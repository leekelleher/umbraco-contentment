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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class JsonDataListSource : IDataListSource
    {
        private readonly ILogger<JsonDataListSource> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IIOHelper _ioHelper;

        public JsonDataListSource(
            ILogger<JsonDataListSource> logger,
            IHostingEnvironment hostingEnvironment,
            IIOHelper ioHelper)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _ioHelper = ioHelper;
        }

        public string Name => "JSON Data";

        public string Description => "Configure JSON data to populate the data source.";

        public string Icon => "icon-brackets";

        public string Group => Constants.Conventions.DataSourceGroups.Data;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new NotesConfigurationField(_ioHelper, $@"<details class=""well well-small"">
<summary><strong>Do you need help with JSONPath expressions?</strong></summary>
<p>This data-source uses Newtonsoft's Json.NET library, with this we are limited to extracting only the 'value' from any key/value-pairs.</p>
<p>If you need assistance with JSONPath syntax, please refer to this resource: <a href=""https://goessner.net/articles/JsonPath/"" target=""_blank""><strong>goessner.net/articles/JsonPath</strong></a>.</p>
<hr>
<p><em>If you are a developer and have ideas on how to extract the `key` (name) from the items, please do let me know on <a href=""{Constants.Package.RepositoryUrl}/issues/40"" target=""_blank""><strong>GitHub issue: #40</strong></a>.</em></p>
</details>", true),
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
            { "url", "https://leekelleher.com/umbraco/contentment/data.json" },
            { "itemsJsonPath", "$[*]" },
            { "nameJsonPath", "$.name" },
            { "valueJsonPath", "$.value" },
            { "iconJsonPath", "$.icon" },
            { "descriptionJsonPath", "$.description" },
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var url = config.GetValueAs("url", string.Empty);

            if (string.IsNullOrWhiteSpace(url) == true)
            {
                return Enumerable.Empty<DataListItem>();
            }

            var json = GetJson(url);

            if (json == null)
            {
                return Enumerable.Empty<DataListItem>();
            }

            var itemsJsonPath = config.GetValueAs("itemsJsonPath", string.Empty);

            if (string.IsNullOrWhiteSpace(itemsJsonPath) == true)
            {
                return Enumerable.Empty<DataListItem>();
            }

            try
            {
                var tokens = json.SelectTokens(itemsJsonPath);

                if (tokens.Any() == false)
                {
                    _logger.LogWarning($"The JSONPath '{itemsJsonPath}' did not match any items in the JSON.");
                    return Enumerable.Empty<DataListItem>();
                }

                // TODO: [UP-FOR-GRABS] How would you get the string-value from a "key"?
                // This project https://github.com/s3u/JSONPath supports "~" to retrieve keys. However this is not in the original jsonpath-specs.
                // We could implement something similar, which checks the JsonPaths for a ~, and the we'll code-extract the keys. However this is a somewhat shady solution.

                var nameJsonPath = config.GetValueAs("nameJsonPath", string.Empty);
                var valueJsonPath = config.GetValueAs("valueJsonPath", string.Empty);
                var iconJsonPath = config.GetValueAs("iconJsonPath", string.Empty);
                var descriptionJsonPath = config.GetValueAs("descriptionJsonPath", string.Empty);

                var items = new List<DataListItem>();

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
                        _logger.LogWarning($"The JSONPath '{nameJsonPath}' did not match a 'name' in the item JSON.");
                    }

                    // If value is missing we'll skip this specific item and log as a warning
                    if (value == null)
                    {
                        _logger.LogWarning($"The JSONPath '{valueJsonPath}' did not match a 'value' in the item XML. The item was skipped.");
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

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding items in the JSON. Please check the syntax of your JSONPath expressions.");
            }

            return Enumerable.Empty<DataListItem>();
        }

        private JToken GetJson(string url)
        {
            var content = string.Empty;

            if (url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) == true)
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
                    _logger.LogError(ex, $"Unable to fetch remote data from URL: {url}");
                }
            }
            else
            {
                // assume local file
                var path = _hostingEnvironment.MapPathWebRoot(url);
                if (File.Exists(path) == true)
                {
                    content = File.ReadAllText(path);
                }
                else
                {
                    _logger.LogError(new FileNotFoundException(), $"Unable to find the local file path: {url}");
                    return null;
                }
            }

            if (string.IsNullOrWhiteSpace(content) == true)
            {
                _logger.LogWarning($"The contents of '{url}' was empty. Unable to process JSON data.");

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
                _logger.LogError(ex, $"Error parsing string to JSON: {trimmed}");
            }

            return default;
        }
    }
}
