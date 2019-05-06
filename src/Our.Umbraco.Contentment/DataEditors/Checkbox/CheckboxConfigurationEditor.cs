using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class CheckboxConfigurationEditor : ConfigurationEditor
    {
        public CheckboxConfigurationEditor()
            : base()
        {
            Fields.Add(
                Constants.Conventions.ConfigurationEditors.ShowInline,
                "Show label inline?",
                "Select to show the property's label and description inline next to the checkbox.<br><br>This option will make the checkbox field span the full width of the editor.",
                "boolean");
        }
    }
}
