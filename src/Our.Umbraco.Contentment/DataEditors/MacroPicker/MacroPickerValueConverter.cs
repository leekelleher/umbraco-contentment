using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Models;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class MacroPickerValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(PublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(MacroPickerDataEditor.DataEditorAlias);

        public override PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType) => PropertyCacheLevel.Element;

        public override Type GetPropertyValueType(PublishedPropertyType propertyType) => typeof(IEnumerable<PartialViewMacroModel>);

        public override object ConvertSourceToIntermediate(IPublishedElement owner, PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source is string value)
            {
                return JsonConvert.DeserializeObject<IEnumerable<MacroPickerModel>>(value);
            }

            return base.ConvertSourceToIntermediate(owner, propertyType, source, preview);
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, PublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if (inter is IEnumerable<MacroPickerModel> items)
            {
                return items.Select(x => new PartialViewMacroModel(default, default, x.Alias, x.Name, x.Parameters)).ToList();
            }

            return base.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
        }
    }
}
