using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class RenderMacroConfigurationEditor : ConfigurationEditor
    {
        public RenderMacroConfigurationEditor()
            : base()
        {
            Fields.Add(
                "macro",
                "Macro",
                "Select and configure the macro to be displayed.",
                IOHelper.ResolveUrl(MacroPickerDataEditor.DataEditorViewPath),
                new Dictionary<string, object> { { Constants.Conventions.ConfigurationEditors.MaxItems, 1 } });
            Fields.AddHideLabel();
        }
    }
}
