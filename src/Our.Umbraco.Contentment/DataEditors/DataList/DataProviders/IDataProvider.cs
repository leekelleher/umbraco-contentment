using System.Collections.Generic;

namespace Our.Umbraco.Contentment.DataEditors
{
    public interface IDataProvider
    {
        string Name { get; }

        string Description { get; }

        string Icon { get; }

        Dictionary<string, string> GetItems();
    }
}
