using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue | EditorType.MacroParameter,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Integer,
        Group = "Display",
        Icon = "icon-readonly")]
    public class NotesDataEditor : DataEditor
    {
        internal const string DataEditorAlias = "Our.Umbraco.Contentment.Notes";
        internal const string DataEditorName = "[Contentment] Notes";
        internal const string DataEditorViewPath = "~/App_Plugins/Contentment/data-editors/notes.html";

        public NotesDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new NotesConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new HideLabelDataValueEditor(Attribute);
    }
}
