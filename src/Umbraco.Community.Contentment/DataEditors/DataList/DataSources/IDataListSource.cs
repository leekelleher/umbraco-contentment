using System.Collections.Generic;

namespace Umbraco.Community.Contentment.DataEditors
{
    public interface IDataListSource : IContentmentListItem
    {
        IEnumerable<DataListItem> GetItems();
    }
}
