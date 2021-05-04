/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;


namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class TextInputValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(TextInputDataEditor.DataEditorAlias);

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(string);

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            return source?.ToString() ?? string.Empty;
        }
    }
}
