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
        Group = "Picker",
        Icon = "icon-science")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
    public class EntityPickerDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.EntityPicker";
        internal const string DataEditorName = "[Contentment] Entity Picker";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/entity-picker.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/entity-picker.js";

        public EntityPickerDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new EntityPickerConfigurationEditor();
    }
}
