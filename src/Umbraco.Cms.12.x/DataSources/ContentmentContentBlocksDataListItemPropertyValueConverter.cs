using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Strings;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;
using Umbraco.Web;

namespace Umbraco.Cms.v11_x.DataSources
{
    public class ContentmentContentBlocksDataListItemPropertyValueConverter : IDataListItemPropertyValueConverter
    {
        private readonly IShortStringHelper _shortStringHelper;

        public ContentmentContentBlocksDataListItemPropertyValueConverter(IShortStringHelper shortStringHelper)
        {
            _shortStringHelper = shortStringHelper;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias == "Umbraco.Community.Contentment.ContentBlocks";
        }

        public IEnumerable<DataListItem> ConvertTo(IPublishedProperty property)
        {
            var value = property.GetValue() as List<IPublishedElement>;

            if (value?.Any() == true)
            {
                return value.Select(x => new DataListItem
                {
                    Name = x.ContentType.Alias.SplitPascalCasing(_shortStringHelper).ToFriendlyName(),
                    Value = x.Key.ToString(),
                    Icon = x.ContentType.GetIcon(),
                    Description = string.Join(", ", x.Properties.Where(p => p.PropertyType.ModelClrType == typeof(string)).Select(p => p.GetValue()?.ToString()?.StripHtml())),
                });
            }

            return Enumerable.Empty<DataListItem>();
        }
    }
}
