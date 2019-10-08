/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class BytesConfigurationEditor : ConfigurationEditor
    {
        internal const string Decimals = "decimals";
        internal const string Filter = "filter";
        internal const string Format = "format";
        internal const string Kilo = "kilo";

        public BytesConfigurationEditor()
        {
            Fields.Add(
                Kilo,
                "Kilobytes?",
                "How many bytes do you prefer in your kilobyte?",
                IOHelper.ResolveUrl(RadioButtonListDataEditor.DataEditorViewPath),
                new Dictionary<string, object>
                {
                    { RadioButtonListConfigurationEditor.Items, new[]
                        {
                            new DataListItem { Name = "1000 bytes", Value = "1000", Description = "The modern standard for a kilobyte is <strong>1000 bytes</strong> (decimal)." },
                            new DataListItem { Name = "1024 bytes", Value = "1024", Description = "Computationally, there are <strong>1024 bytes</strong> (binary). Today, this is known as a kibibyte." },
                        }
                    },
                    { RadioButtonListConfigurationEditor.DefaultValue, "1024" },
                });

            Fields.Add(
                Decimals,
                "Decimal places",
                "How many decimal places would you like?",
                IOHelper.ResolveUrl("~/umbraco/views/propertyeditors/slider/slider.html"),
                new Dictionary<string, object>
                {
                    { "initVal1", 2 },
                    { "minVal", 0 },
                    { "maxVal", 10 },
                    { "step", 1 }
                });
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.ContainsKey(Filter) == false)
            {
                config.Add(Filter, "formatBytes");

                if (config.ContainsKey(Format) == false && config.ContainsKey(Kilo) && config.ContainsKey(Decimals))
                {
                    config.Add(Format, new
                    {
                        kilo = config[Kilo],
                        decimals = config[Decimals]
                    });

                    config.Remove(Kilo);
                    config.Remove(Decimals);
                }
            }

            return config;
        }
    }
}
