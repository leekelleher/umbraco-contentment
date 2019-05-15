/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal static class ConfigurationFieldExtensions
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
            fields.Add(
                Constants.Conventions.ConfigurationEditors.DisableSorting,
                "Disable sorting?",
                "Select to disable sorting of the items.",
                "boolean");
        }

        public static void AddHideLabel(this List<ConfigurationField> fields)
        {
            fields.Add(
                Constants.Conventions.ConfigurationEditors.HideLabel,
                "Hide label?",
                "Select to hide the label and have the editor take up the full width of the panel.",
                "boolean");
        }

        public static void AddMaxItems(this List<ConfigurationField> fields)
        {
            fields.Add(
                Constants.Conventions.ConfigurationEditors.MaxItems,
                "Max items",
                "Enter the number for the maximum items allowed.<br>Use '0' for an unlimited amount.",
                "number");
        }
    }
}
