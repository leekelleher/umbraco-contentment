using ClientDependency.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Integer,
        Group = "Display",
        Icon = "icon-settings-alt")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
    public class RenderMacroDataEditor: DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.RenderMacro";
        internal const string DataEditorName = "[Contentment] Render Macro";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/render-macro.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/render-macro.js";

        public RenderMacroDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new RenderMacroConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new HideLabelDataValueEditor(Attribute);
    }
}
