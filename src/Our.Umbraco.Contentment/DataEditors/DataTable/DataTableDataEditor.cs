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
        ValueType = ValueTypes.String,
        Group = "Lists",
        Icon = "icon-bulleted-list")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataEditorJsPath)]
    public class DataTableDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.DataTable";
        internal const string DataEditorName = "[Contentment] Data Table";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/data-table.html";
        internal const string DataEditorJsPath = "~/App_Plugins/Contentment/data-editors/data-table.js";

        public DataTableDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new DataTableConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new HideLabelDataValueEditor(Attribute);
    }
}
