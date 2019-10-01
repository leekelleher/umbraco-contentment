/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class HideLabelDataValueEditor : DataValueEditor
    {
        public HideLabelDataValueEditor(DataEditorAttribute attribute)
            : base(attribute)
        { }

        public override object Configuration
        {
            get => base.Configuration;
            set
            {
                base.Configuration = value;

                if (value is Dictionary<string, object> config && config.ContainsKey(HideLabelConfigurationField.HideLabelAlias))
                {
                    // NOTE: This is how NestedContent handles this in core. Feels like code-smell to me. [LK:2019-05-03]
                    // https://github.com/umbraco/Umbraco-CMS/blob/release-8.0.0/src/Umbraco.Web/PropertyEditors/NestedContentPropertyEditor.cs#L80
                    // I don't think "display logic" should be done inside a property's setter method.
                    // So where is the best place to do this? I would have assumed `ToEditor`, but the `Configuration` is empty?!
                    HideLabel = config[HideLabelConfigurationField.HideLabelAlias].TryConvertTo<bool>().Result;
                }
            }
        }
    }
}
