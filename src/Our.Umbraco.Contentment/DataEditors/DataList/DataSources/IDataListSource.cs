using System.Collections.Generic;

namespace Our.Umbraco.Contentment.DataEditors
{
    public interface IDataListSource : IConfigurationEditorItem
    {
        Dictionary<string, string> GetItems();
    }
}
