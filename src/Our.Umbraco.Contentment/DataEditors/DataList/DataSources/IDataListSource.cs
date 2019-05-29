using System.Collections.Generic;

namespace Our.Umbraco.Contentment.DataEditors
{
    public interface IDataListSource : IConfigurationEditorItem
    {
        IEnumerable<DataListItemModel> GetItems();
    }
}
