using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal static class ConfigurationFieldExtentions
    {
        public static List<ConfigurationField> Add(
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

            return fields;
        }

        public static List<ConfigurationField> AddHideLabel(this List<ConfigurationField> fields)
        {
            return fields.Add(
                Constants.Conventions.ConfigurationEditors.HideLabel,
                "Hide label?",
                "Select to hide the label and have the editor take up the full width of the panel.",
                "boolean");
        }

        public static List<ConfigurationField> AddMaxItems(this List<ConfigurationField> fields)
        {
            return fields.Add(
                Constants.Conventions.ConfigurationEditors.MaxItems,
                "Max items",
                "Enter the number for the maximum items allowed. Use '0' for an unlimited amount.",
                "number");
        }
    }
}
