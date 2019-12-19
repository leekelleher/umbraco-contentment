﻿/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class JsonDataListSource : IDataListSource
    {
        private readonly ILogger _logger;

        public JsonDataListSource(ILogger logger)
        {
            _logger = logger;
        }

        public string Name => "JSON";

        public string Description => "Configure the data source to use JSON data.";

        public string Icon => "icon-brackets";

        public Dictionary<string, object> DefaultValues => default;

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

        public IEnumerable<DataListItem> GetItems()
        {
            var items = new List<DataListItem>();

            var json = GetJson();

            if (json == null)
                return items;

            foreach (JProperty token in json.Children())
            {
                items.Add(new DataListItem
                {
                    Name = token.Value?.ToString(),
                    Value = token.Name
                });
            }

            return items;
        }

        private JObject GetJson()
        {
            if (string.IsNullOrWhiteSpace(Url))
                return null;

            var json = default(JObject);

            if (Url.InvariantStartsWith("http"))
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        var response = client.DownloadString(Url);
                        if (string.IsNullOrWhiteSpace(response) == false)
                        {
                            json = JObject.Parse(response);
                        }
                    }
                }
                catch (WebException ex)
                {
                    Current.Logger.Error<JsonDataListSource>(ex, "Unable to fetch remote data.");
                }
            }
            else
            {
                // assume local file
                var path = IOHelper.MapPath(Url);
                if (File.Exists(path))
                {
                    var contents = File.ReadAllText(path);
                    if (string.IsNullOrWhiteSpace(contents) == false)
                    {
                        json = JObject.Parse(contents);
                    }
                }
                else
                {
                    _logger.Warn<JsonDataListSource>("Unable to find the local file path.");
                }
            }

            return json;
        }

        class JsonNotesConfigurationField : NotesConfigurationField
        {
            // TODO: [LK:2019-07-19] Explain how these JSONPath queries work, (as I have no idea myself!)
            public JsonNotesConfigurationField()
                : base(@"<p class=""alert alert-success""><strong>A note about JSONPath expressions.</strong><br>
[add info about JSONPath, links, etc.]</p>", true)
            { }
        }
    }
}
