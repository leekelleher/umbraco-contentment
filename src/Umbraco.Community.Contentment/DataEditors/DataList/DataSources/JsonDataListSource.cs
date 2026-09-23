/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

// NOTE: Uses Newtonsoft.Json for JsonPath support; System.Text.Json has no JSONPath query engine. [LK]
namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class JsonDataListSource : DataListToDataPickerSourceBridge, IContentmentDataSource, IContentmentListTemplateItem
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly IHttpClientFactory _httpClientFactory;

        [ActivatorUtilitiesConstructor]
        public JsonDataListSource(
            IWebHostEnvironment webHostEnvironment,
            IHttpClientFactory httpClientFactory)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpClientFactory = httpClientFactory;
        }

        [Obsolete("To be removed in Contentment 8.0")]
        public JsonDataListSource(IWebHostEnvironment webHostEnvironment)
            : this(webHostEnvironment, StaticServiceProvider.Instance.GetRequiredService<IHttpClientFactory>())
        { }

        public override string Name => "JSON Data";

        public string? NameTemplate => default;

        public override string Description => "Configure JSON data to populate the data source.";

        public string? DescriptionTemplate => "{= url }";

        public override string Icon => "icon-brackets";

        public override string Group => Constants.Conventions.DataSourceGroups.Data;

        public override OverlaySize OverlaySize => OverlaySize.Medium;

        public override IEnumerable<ContentmentConfigurationField> Fields => new[]
        {
            new NotesConfigurationField(@"<details class=""well"">
<summary><strong>Do you need help with JSONPath expressions?</strong></summary>
<p>This data-source uses Newtonsoft's Json.NET library, with this we are limited to extracting only the 'value' from any key/value-pairs.</p>
<p>If you need assistance with JSONPath syntax, please refer to this resource: <a href=""https://goessner.net/articles/JsonPath/"" target=""_blank""><strong>goessner.net/articles/JsonPath</strong></a>.</p>
</details>", true),
            new ContentmentConfigurationField
            {
                Key = "url",
                Name = "URL",
                Description = "Enter the URL of the JSON data source.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "itemsJsonPath",
                Name = "Items JSONPath",
                Description = "Enter the JSONPath expression to select the items from the JSON data source.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "nameJsonPath",
                Name = "Name JSONPath",
                Description = "Enter the JSONPath expression to select the name from the item.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "valueJsonPath",
                Name = "Value JSONPath",
                Description = "Enter the JSONPath expression to select the value (key) from the item.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "iconJsonPath",
                Name = "Icon JSONPath",
                Description = "<em>(optional)</em> Enter the JSONPath expression to select the icon from the item.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "descriptionJsonPath",
                Name = "Description JSONPath",
                Description = "<em>(optional)</em> Enter the JSONPath expression to select the description from the item.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
        };

        public override Dictionary<string, object> DefaultValues => new()
        {
            { "url", "https://leekelleher.com/umbraco/contentment/data.json" },
            { "itemsJsonPath", "$[*]" },
            { "nameJsonPath", "$.name" },
            { "valueJsonPath", "$.value" },
            { "iconJsonPath", "$.icon" },
            { "descriptionJsonPath", "$.description" },
        };

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
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
                    // The JSONPath '{itemsJsonPath}' did not match any items in the JSON.
                    return Enumerable.Empty<DataListItem>();
                }

                var nameJsonPath = config.GetValueAs("nameJsonPath", string.Empty) ?? string.Empty;
                var valueJsonPath = config.GetValueAs("valueJsonPath", string.Empty) ?? string.Empty;
                var iconJsonPath = config.GetValueAs("iconJsonPath", string.Empty);
                var descriptionJsonPath = config.GetValueAs("descriptionJsonPath", string.Empty);

                var items = new List<DataListItem>();

                foreach (var token in tokens)
                {
                    var value = token.SelectToken(valueJsonPath);

                    if (value == null)
                    {
                        // If value is missing we'll skip this specific item and log as a warning.
                        continue;
                    }

                    var name = token.SelectToken(nameJsonPath) ?? value;

                    var icon = string.IsNullOrEmpty(iconJsonPath) == false
                        ? token.SelectToken(iconJsonPath)
                        : null;

                    var description = string.IsNullOrEmpty(descriptionJsonPath) == false
                        ? token.SelectToken(descriptionJsonPath)
                        : null;

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
            catch (Exception)
            {
                // Error finding items in the JSON. Please check the syntax of your JSONPath expressions.
            }

            return Enumerable.Empty<DataListItem>();
        }

        private JToken? GetJson(string url)
        {
            var content = string.Empty;

            if (url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                try
                {
                    using var client = _httpClientFactory.CreateClient();
                    using var request = new HttpRequestMessage(HttpMethod.Get, url);
                    using var response = client.Send(request);

                    response.EnsureSuccessStatusCode();

                    content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
                {
                    // Unable to fetch remote data from URL: '{url}'.
                }
            }
            else
            {
                // assume local file
                var path = _webHostEnvironment.MapPathWebRoot(url);
                if (File.Exists(path) == true)
                {
                    content = File.ReadAllText(path);
                }
                else
                {
                    // Unable to find the local file path: '{url}'.
                    return default;
                }
            }

            if (string.IsNullOrWhiteSpace(content) == true)
            {
                // The contents of '{url}' was empty. Unable to process JSON data.
                return default;
            }

            try
            {
                // Deserialize to a JToken, for general purposes.
                // Inspiration taken from StackOverflow: https://stackoverflow.com/a/38560188/12787
                return JToken.Parse(content);
            }
            catch (Exception)
            {
                // Error parsing string to JSON.
                // Wondering if a `JToken.TryParse()` should be a thing? [LK]
            }

            return default;
        }
    }
}
