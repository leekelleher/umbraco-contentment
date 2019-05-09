using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Models.ContentEditing;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class MacroPickerConfigurationEditor : ConfigurationEditor
    {
        public MacroPickerConfigurationEditor()
            : base()
        {
            Fields.Add(
                "allowedMacros",
                "Allowed Macros",
                "Restrict the macros that can be picked.",
                IOHelper.ResolveUrl(EntityPickerDataEditor.DataEditorViewPath),
                new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationEditors.EntityType, nameof(UmbracoEntityTypes.Macro) },
                    { "semisupportedTypes", new[]{ nameof(UmbracoEntityTypes.Macro) } }
                });
            Fields.AddMaxItems();
            Fields.AddDisableSorting();
            Fields.AddHideLabel();
        }
    }
}
