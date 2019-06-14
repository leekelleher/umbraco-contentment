/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class CascadingDropdownListConfigurationEditor : ConfigurationEditor
    {
        public const string APIs = "apis";

        public CascadingDropdownListConfigurationEditor()
            : base()
        {
            Fields.Add(
                APIs,
                nameof(APIs),
                "[Add friendly description]", // TODO: [LK:2019-06-14] Add friendly description
                IOHelper.ResolveUrl("~/umbraco/views/propertyeditors/multipletextbox/multipletextbox.html"),
                new Dictionary<string, object>
                {
                    { "min", 0 },
                    { "max", 0 },
                });
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            // NOTE: The MultipleTextbox editor stores the value as an object-array, whereas we only need a string-array. [LK]
            if (config.TryGetValue(APIs, out var obj) && obj is JArray array && array.Count > 0)
            {
                config[APIs] = array.Select(x => x.Value<string>("value"));
            }

            return config;
        }
    }
}
