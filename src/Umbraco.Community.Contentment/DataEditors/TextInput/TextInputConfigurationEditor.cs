/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class TextInputConfigurationEditor : ConfigurationEditor
    {
        public const string Autocomplete = "autocomplete";
        public const string Items = "items";
        public const string MaxChars = "maxChars";
        public const string PlaceholderText = "placeholderText";

        public TextInputConfigurationEditor(ConfigurationEditorUtility utility)
            : base()
        {
            Fields.Add(
                PlaceholderText,
                "Placeholder text",
                "Add placeholder text for the text input. This is to be used as instructional information, not as a default value.",
                "textstring");

            Fields.Add(
                "autocomplete",
                "Enable autocomplete?",
                "Select to enable autocomplete functionality on the text input.",
                "boolean");

            Fields.Add(
                "maxChars",
                "Maximum allowed characters",
                "Enter the maximum number of characters allowed for the text input.<br>The default limit is 500 characters.",
                IOHelper.ResolveUrl(NumberInputDataEditor.DataEditorViewPath));

            //Fields.Add(
            //    "prepend",
            //    "Prepend Icon",
            //    "[Add friendly description]",
            //    IOHelper.ResolveUrl(IconPickerDataEditor.DataEditorViewPath),
            //    new Dictionary<string, object>
            //    {
            //        { DefaultIconConfigurationField.DefaultIcon, string.Empty },
            //        { IconPickerSizeConfigurationField.Size, IconPickerSizeConfigurationField.Large }
            //    });

            //Fields.Add(
            //    "append",
            //    "Append Icon",
            //    "[Add friendly description]",
            //    IOHelper.ResolveUrl(IconPickerDataEditor.DataEditorViewPath),
            //    new Dictionary<string, object>
            //    {
            //        { DefaultIconConfigurationField.DefaultIcon, string.Empty },
            //        { IconPickerSizeConfigurationField.Size, IconPickerSizeConfigurationField.Large }
            //    });

            var dataSources = utility.GetConfigurationEditors<IDataListSource>();

            Fields.Add(
                Items,
                "Data list",
                "<em>(optional)</em> Select and configure the data source to provide a HTML5 &lt;datalist&gt; for this text input.",
                IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { OverlaySizeConfigurationField.OverlaySize, OverlaySizeConfigurationField.Small },
                    { ConfigurationEditorConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) },
                    { ConfigurationEditorConfigurationEditor.Items, dataSources },
                    { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.False },
                    { DisableSortingConfigurationField.DisableSorting, Constants.Values.True },
                    { MaxItemsConfigurationField.MaxItems, 1 },
                });
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValue(Items, out var items) && items is JArray array && array.Count > 0)
            {
                var item = array[0];

                var type = TypeFinder.GetTypeByName(item.Value<string>("type"));
                if (type != null)
                {
                    var serializer = JsonSerializer.CreateDefault(new Serialization.ConfigurationFieldJsonSerializerSettings());

                    var source = item["value"].ToObject(type, serializer) as IDataListSource;
                    var options = source?.GetItems() ?? Enumerable.Empty<DataListItem>();

                    config[Items] = options;
                }
            }

            return config;
        }
    }
}
