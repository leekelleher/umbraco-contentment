using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class DropdownConfigurationEditor : ConfigurationEditor
    {
        public DropdownConfigurationEditor()
            : base()
        {
            var listFields = new[]
            {
                new ConfigurationField
                {
                    Key = "label",
                    Name = "Label",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "value",
                    Name = "Value",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "enabled",
                    Name = "Enabled",
                    View = "boolean",
                    Config = new Dictionary<string, object> { { "default", "1" } }
                },
            };

            Fields.Add(
                "items",
                "Options",
                "Configure the option items for the dropdown list.",
                IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { "fields", listFields },
                    { Constants.Conventions.ConfigurationEditors.MaxItems, "0" },
                    { Constants.Conventions.ConfigurationEditors.DisableSorting, "0" },
                    { "usePrevalueEditors", "0" }
                });
            Fields.AddHideLabel();
        }
    }
}
