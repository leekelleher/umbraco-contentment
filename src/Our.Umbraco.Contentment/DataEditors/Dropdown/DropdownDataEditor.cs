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
        ValueType = ValueTypes.String,
        Group = "Lists",
        Icon = "icon-indent")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
    public class DropdownDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.Dropdown";
        internal const string DataEditorName = "[Contentment] Dropdown";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/dropdown.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/dropdown.js";

        public DropdownDataEditor(ILogger logger)
            : base(logger)
        { }

        // TODO: Add a prevalue editor for adding label/value/disabled items [LK]
    }
}
