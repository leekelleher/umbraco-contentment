/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
#if NET472
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class IconPickerValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(IconPickerDataEditor.DataEditorAlias);

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(string);
    }
}
