using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class MacroPickerConfigurationEditor : ConfigurationEditor
    {
        public MacroPickerConfigurationEditor()
            : base()
        {
            Fields
                .AddHideLabel()
                .AddMaxItems();
        }
    }
}
