/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;


namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class RenderMacroValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(RenderMacroDataEditor.DataEditorAlias);

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => PropertyCacheLevel.None;

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(object);

        public override bool? IsValue(object value, PropertyValueLevel level) => false;
    }
}
