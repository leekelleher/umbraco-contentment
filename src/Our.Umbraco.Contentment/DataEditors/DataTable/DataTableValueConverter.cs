using System;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class DataTableValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(PublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(DataTableDataEditor.DataEditorAlias);

        public override PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType) => PropertyCacheLevel.Element;

        public override Type GetPropertyValueType(PublishedPropertyType propertyType) => typeof(JArray);

        public override object ConvertSourceToIntermediate(IPublishedElement owner, PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source is string value)
            {
                return JArray.Parse(value);
            }

            return base.ConvertSourceToIntermediate(owner, propertyType, source, preview);
        }
    }
}
