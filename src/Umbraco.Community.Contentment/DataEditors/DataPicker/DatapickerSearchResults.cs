using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.Community.Contentment.DataEditors
{
    public class DataPickerSearchResults 
    {
        public int TotalPages { get; set; } = -1;

        public IEnumerable<DataPickerItem> Items { get; set; } = Array.Empty<DataPickerItem>();
    }
}
