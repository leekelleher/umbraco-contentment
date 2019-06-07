/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class ByteSizeConfigurationEditor : ConfigurationEditor
    {
        public const string Filter = "filter";
        public const string Format = "format";

        public ByteSizeConfigurationEditor()
        {
            Fields.Add(
                Format,
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
                // NOTE: Unfortunately this wont work with v8.0.2, as the ReadOnlyValueController is expecting an array object.
                // https://github.com/umbraco/Umbraco-CMS/blob/release-8.0.2/src/Umbraco.Web.UI.Client/src/views/propertyeditors/readonlyvalue/readonlyvalue.controller.js#L18-L22
                // TODO: [LK:2019-06-07] Patch supplied to Umbraco: https://github.com/umbraco/Umbraco-CMS/pull/5615
                config.Add(Filter, "formatBytes");
            }

            return config;
        }
    }
}
