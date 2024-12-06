/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Text.Json;
using System.Text.Json.Nodes;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class TextInputConfigurationEditor : ConfigurationEditor
    {
        private readonly ConfigurationEditorUtility _utility;

        public TextInputConfigurationEditor(ConfigurationEditorUtility utility)
            : base()
        {
            _utility = utility;

            //var dataSources = new List<ConfigurationEditorModel>(_utility.GetConfigurationEditorModels<IDataListSource>());

            DefaultConfiguration.Add("inputType", "text");
        }

        public override IDictionary<string, object> ToValueEditor(IDictionary<string, object> configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValueAs(Constants.Conventions.ConfigurationFieldAliases.Items, out JsonArray? array) == true &&
                array?.Count > 0 &&
                array[0] is JsonObject item &&
                item.GetValueAsString("key") is string key)
            {
                var source = _utility.GetConfigurationEditor<IDataListSource>(key);
                if (source != null)
                {
                    var sourceConfig = item["value"]?.Deserialize<Dictionary<string, object>>();
                    if (sourceConfig is not null)
                    {
                        var items = source?.GetItems(sourceConfig) ?? Array.Empty<DataListItem>();

                        config[Constants.Conventions.ConfigurationFieldAliases.Items] = items;
                    }
                }
            }

            return config;
        }
    }
}
