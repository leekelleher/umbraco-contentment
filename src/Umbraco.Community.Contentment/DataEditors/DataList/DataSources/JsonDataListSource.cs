/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

// TODO
// How would you get the string-value from a "key"?
// This project https://github.com/s3u/JSONPath supports "~" to retrieve keys. However this is not in the original jsonpath-specs.
// We could implement something similar, which checks the JsonPaths for a ~, and the we'll code-extract the keys. However this is a somewhat shady solution.

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class JsonDataListSource : IDataListSource
    {
        private readonly ILogger _logger;

        public JsonDataListSource(ILogger logger)
        {
            _logger = logger;
        }

        public string Name => "JSON Data";

        public string Description => "Configure the data source (file or url) to use JSON data. Data retrieved from urls are set to UTF-8 encoding.";

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

            var json = GetJson();

            if (json == null)
                return items;

            try
            {
                var jsonItems = json.SelectTokens(ItemsJsonPath);

                // if either all names are null or values are null, then we'll return empty array and log an exception
                var allNameNull = true;
                var allValueNull = true;

                foreach (var item in jsonItems)
                {
                    var name = item.SelectToken(NameJsonPath);
                    var value = item.SelectToken(ValueJsonPath);
                    var icon = string.IsNullOrEmpty(IconJsonPath) ? null : item.SelectToken(IconJsonPath);
                    var description = string.IsNullOrEmpty(DescriptionJsonPath) ? null : item.SelectToken(DescriptionJsonPath);

                    // How should we log if either name or value is empty? Note that empty or missing values are totally legal according to json
                    if (name == null) _logger.Warn<JsonDataListSource>("Contentment | Logging: No 'name' was found using JSONPath: " + NameJsonPath);
                    if (value == null) _logger.Warn<JsonDataListSource>("Contentment | Logging: No 'value' was found using JSONPath: " + ValueJsonPath);

                    allNameNull = allNameNull && (name == null); // Will flip first time name is not null
                    allValueNull = allValueNull && (value == null); // Will flip first time value is not null

                    items.Add(new DataListItem
                    {
                        Name = name?.ToString() ?? "",
                        Value = value?.ToString() ?? "",
                        Icon = icon?.ToString() ?? "",
                        Description = description?.ToString() ?? ""
                    });
                }

                if (allNameNull)
                {
                    _logger.Error<JsonDataListSource>("Contentment | Logging: No 'names' were found in json data. Consider changing your JSONPath. ");
                    // We return empty list to emphasize error
                    items.Clear();
                }
                if (allValueNull)
                {
                    _logger.Error<JsonDataListSource>("Contentment | Logging: No 'values' were found in json data. Consider changing your JSONPath. ");
                    // We return empty list to emphasize error
                    items.Clear();
                }

            }
            catch (Exception ex)
            {
                _logger.Error<JsonDataListSource>(ex, "Error finding nodes in the JSON object. Check the syntax of your JSON Paths.");
            }


            return items;
        }

        private JToken GetJson()
        {
            if (string.IsNullOrWhiteSpace(Url))
                return null;

            var content = "";
            var json = default(JToken);

            if (Url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        client.Encoding = Encoding.UTF8;
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

            // Here the question is what to deserialize the json-string to.
            // I was inspired by a post I found on StackOverflow (https://stackoverflow.com/questions/38558844/jcontainer-jobject-jtoken-and-linq-confusion):
            //
            // Here's the basic rule of thumb:
            //
            // If you know you have an object(denoted by curly braces { and } in JSON), use JObject
            // If you know you have an array or list(denoted by square brackets[and]), use JArray
            // If you know you have a primitive value, use JValue
            // If you don't know what kind of token you have, or want to be able to handle any of the above in a general way, use JToken. You can then check its Type property to determine what kind of token it is and cast it appropriately.

            if (string.IsNullOrWhiteSpace(content) == false)
            {
                try
                {
                    json = JToken.Parse(content);
                }
                catch (Exception ex)
                {
                    _logger.Error<JsonDataListSource>(ex, "Error parsing string into json: " + content.Substring(0, Math.Min(400, content.Length)));
                    return null;
                }

            }
            else
            {
                _logger.Warn<JsonDataListSource>($"JsonContent ({Url}) was empty.");
                return null;
            }
            return json;
        }

        class JsonNotesConfigurationField : NotesConfigurationField
        {
            // TODO: [LK:2019-07-19] Explain how these JSONPath queries work, (as I have no idea myself!)
            public JsonNotesConfigurationField()
                : base(@"<p class=""alert alert-success""><strong>Help with your JSONPath?</strong><br>
Using Newtonsoft we're limitted to only extracting the 'value' of the key/value-pair attributes.<br /><br />
Please refer to this resource for more information: <a href='https://goessner.net/articles/JsonPath/' target='_blank'>https://goessner.net/articles/JsonPath/</a>
</p>", true)
            { }
        }
    }
}
