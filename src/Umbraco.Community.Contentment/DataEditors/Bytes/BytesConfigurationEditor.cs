/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class BytesConfigurationEditor : ConfigurationEditor
    {
        internal const string Decimals = "decimals";
        internal const string Filter = "filter";
        internal const string Format = "format";
        internal const string Kilo = "kilo";

        public BytesConfigurationEditor(IIOHelper ioHelper)
        {
            Fields.Add(new ConfigurationField
            {
                Key = Kilo,
                Name = "Kilobytes?",
                Description = "How many bytes do you prefer in your kilobyte?",
                View = ioHelper.ResolveRelativeOrVirtualUrl(RadioButtonListDataListEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "1000 bytes", Value = "1000", Description = "The modern standard for a kilobyte is <strong>1000 bytes</strong> (decimal)." },
                            new DataListItem { Name = "1024 bytes", Value = "1024", Description = "Computationally, there are <strong>1024 bytes</strong> (binary). Today, this is known as a kibibyte." },
                        }
                    },
                    { ShowDescriptionsConfigurationField.ShowDescriptions, Constants.Values.True },
                    { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, "1024" },
                }
            });

            Fields.Add(new ConfigurationField
            {
                Key = Decimals,
                Name = "Decimal places",
                Description = "How many decimal places would you like?",
                View = ioHelper.ResolveRelativeOrVirtualUrl("~/umbraco/views/propertyeditors/slider/slider.html"),
                Config = new Dictionary<string, object>
                {
                    { "initVal1", 2 },
                    { "minVal", 0 },
                    { "maxVal", 10 },
                    { "step", 1 }
                }
            });
        }

        public override IDictionary<string, object> ToValueEditor(object? configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.ContainsKey(Filter) == false)
            {
                config.Add(Filter, "formatBytes");

                if (config.ContainsKey(Format) == false && config.ContainsKey(Kilo) == true && config.ContainsKey(Decimals) == true)
                {
                    config.Add(Format, new
                    {
                        kilo = config[Kilo],
                        decimals = config[Decimals]
                    });

                    _ = config.Remove(Kilo);
                    _ = config.Remove(Decimals);
                }
            }

            return config;
        }
    }
}
