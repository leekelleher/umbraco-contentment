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
        ValueType = ValueTypes.Integer,
        Group = "Common",
        Icon = "icon-checkbox")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
    public class CheckboxDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.Checkbox";
        internal const string DataEditorName = "[Contentment] Checkbox";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/checkbox.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/checkbox.js";

        public CheckboxDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new CheckboxConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new CheckboxDataValueEditor(Attribute);
    }
}
