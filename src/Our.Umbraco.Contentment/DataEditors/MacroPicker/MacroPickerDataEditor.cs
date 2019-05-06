using ClientDependency.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue | EditorType.MacroParameter,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Json,
        Group = "picker",
        Icon = "icon-settings-alt")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
    public class MacroPickerDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.MacroPicker";
        internal const string DataEditorName = "[Contentment] Macro Picker";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/macro-picker.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/macro-picker.js";

        public MacroPickerDataEditor(ILogger logger)
            : base(logger)
        {
            DefaultConfiguration.Add(Constants.Conventions.ConfigurationEditors.MaxItems, 0);
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new MacroPickerConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new MacroPickerDataValueEditor(Attribute);
    }
}
