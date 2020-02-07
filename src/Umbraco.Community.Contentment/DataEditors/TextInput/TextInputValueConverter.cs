/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PropertyEditors.ValueConverters;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Core.Composing.HideFromTypeFinder]
    public sealed class TextInputValueConverter : TextStringValueConverter
    {
        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(TextInputDataEditor.DataEditorAlias);
    }
}
