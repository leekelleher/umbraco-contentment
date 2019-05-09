using System;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class CheckboxValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(PublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(CheckboxDataEditor.DataEditorAlias);

        public override PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType) => PropertyCacheLevel.Element;

        public override Type GetPropertyValueType(PublishedPropertyType propertyType) => typeof(bool);
    }
}
