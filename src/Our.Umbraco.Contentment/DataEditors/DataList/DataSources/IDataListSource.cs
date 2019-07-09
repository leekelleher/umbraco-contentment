using System.Collections.Generic;

namespace Our.Umbraco.Contentment.DataEditors
{
    public interface IDataListSource : IContentmentListItem
    {
        IEnumerable<DataListItem> GetItems();
    }
}
