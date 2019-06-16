/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal static partial class ConfigurationFieldExtensions
    {
        public static void Add(
            this List<ConfigurationField> fields,
            string key,
            string name,
            string description,
            string view,
            IDictionary<string, object> config = null)
        {
            fields.Add(new ConfigurationField
            {
                Key = key,
                Name = name,
                Description = description,
                View = view,
                Config = config,
            });
        }

        public static void AddDisableSorting(this List<ConfigurationField> fields)
        {
            fields.Add(new DisableSortingConfigurationField());
        }

        public static void AddHideLabel(this List<ConfigurationField> fields)
        {
            fields.Add(new HideLabelConfigurationField());
        }

        public static void AddMaxItems(this List<ConfigurationField> fields)
        {
            fields.Add(new MaxItemsConfigurationField());
        }
    }
}
