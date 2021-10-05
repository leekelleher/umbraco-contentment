/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class TextInputConfigurationEditor : ConfigurationEditor
    {
        private readonly ConfigurationEditorUtility _utility;

        public TextInputConfigurationEditor(ConfigurationEditorUtility utility)
            : base()
        {
            _utility = utility;

            var dataSources = new List<ConfigurationEditorModel>(_utility.GetConfigurationEditorModels<IDataListSource>());

            Fields.Add(new ConfigurationField
            {
                Key = "placeholderText",
                Name = "Placeholder text",
                Description = "Add placeholder text for the text input.<br>This is to be used as instructional information, not as a default value.",
                View = "textstring",
            });

            Fields.Add(new ConfigurationField
            {
                Key = Constants.Conventions.ConfigurationFieldAliases.Items,
                Name = "Data list",
                Description = "<em>(optional)</em> Select and configure a data source to provide a HTML5 &lt;datalist&gt; for this text input.",
                View = IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>()
                {
                    { Constants.Conventions.ConfigurationFieldAliases.AddButtonLabelKey, "contentment_configureDataSource" },
                    { MaxItemsConfigurationField.MaxItems, 1 },
                    { DisableSortingConfigurationField.DisableSorting, Constants.Values.True },
                    { Constants.Conventions.ConfigurationFieldAliases.OverlayView, IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) },
                    { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
                    { EnableFilterConfigurationField.EnableFilter, dataSources.Count > 10 ? Constants.Values.True : Constants.Values.False },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, dataSources },
                }
            });

            Fields.Add(new ConfigurationField
            {
                Name = "Maximum allowed characters",
                Key = "maxChars",
                Description = "Enter the maximum number of characters allowed for the text input.<br>The default limit is 500 characters.",
                View = IOHelper.ResolveUrl(NumberInputDataEditor.DataEditorViewPath),
            });

            Fields.Add(new ConfigurationField
            {
                Key = "autocomplete",
                Name = "Enable autocomplete?",
                Description = "Select to enable your web-browser's autocomplete functionality on the text input.",
                View = "boolean",
            });

            Fields.Add(new ConfigurationField
            {
                Key = "spellcheck",
                Name = "Enable spellcheck?",
                Description = "Select to enable your web-browser's spellcheck functionality on the text input.",
                View = "boolean",
            });
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValueAs(Constants.Conventions.ConfigurationFieldAliases.Items, out JArray array) == true && array.Count > 0 && array[0] is JObject item)
            {
                // NOTE: Patches a breaking-change. I'd renamed `type` to become `key`.
                if (item.ContainsKey("key") == false && item.ContainsKey("type") == true)
                {
                    item.Add("key", item["type"]);
                    item.Remove("type");
                }

                var source = _utility.GetConfigurationEditor<IDataListSource>(item.Value<string>("key"));
                if (source != null)
                {
                    var sourceConfig = item["value"].ToObject<Dictionary<string, object>>();
                    var items = source?.GetItems(sourceConfig) ?? Array.Empty<DataListItem>();

                    config[Constants.Conventions.ConfigurationFieldAliases.Items] = items;
                }
            }

            return config;
        }
    }
}
