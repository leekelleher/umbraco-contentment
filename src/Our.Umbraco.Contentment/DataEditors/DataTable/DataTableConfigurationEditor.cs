using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class DataTableConfigurationEditor : ConfigurationEditor
    {
        public DataTableConfigurationEditor()
            : base()
        {
            // TODO: Review how this should be populated, whether it be from DocType, Macro, or manually (or all the above?)
            var listFields = new[]
            {
                new ConfigurationField
                {
                    Key = "key",
                    Name = "Key",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "label",
                    Name = "Name",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "view",
                    Name = "Editor",
                    View = IOHelper.ResolveUrl(DropdownDataEditor.DataEditorViewPath),
                    Config = new Dictionary<string, object>
                    {
                        {
                            "items", new[]
                            {
                                new { label = "Textstring", value = "textbox" },
                                new { label = "Checkbox", value = "boolean" },
                            }
                        }
                    }
                },
            };

            Fields.Add(
                "fields",
                "Fields",
                "Configure the editor fields for the data table.",
                IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { "fields", listFields },
                    { Constants.Conventions.ConfigurationEditors.MaxItems, "0" },
                    { Constants.Conventions.ConfigurationEditors.DisableSorting, "0" },
                    { "usePrevalueEditors", "0" }
                });
            Fields.AddMaxItems();
            Fields.AddHideLabel();
            Fields.AddDisableSorting();
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            config.Add("usePrevalueEditors", "0");

            return config;
        }
    }
}
