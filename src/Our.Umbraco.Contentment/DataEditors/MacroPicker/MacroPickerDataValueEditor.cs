using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class MacroPickerDataValueEditor : DataValueEditor
    {
        public MacroPickerDataValueEditor(DataEditorAttribute attribute)
            : base(attribute)
        { }

        public override object Configuration
        {
            get => base.Configuration;
            set
            {
                base.Configuration = value;

                if (value is Dictionary<string, object> config && config.ContainsKey(Constants.Conventions.ConfigurationEditors.HideLabel))
                {
                    // NOTE: This is how NestedContent handles this in core. Looks like a code-smell to me. [LK:2019-05-03]
                    // I don't think "display logic" should be done inside the setter.
                    // Where is the best place to do this? I'd assume `ToEditor`, but the `Configuration` is empty?!
                    HideLabel = config[Constants.Conventions.ConfigurationEditors.HideLabel].TryConvertTo<bool>().Result;
                }
            }
        }
    }
}
