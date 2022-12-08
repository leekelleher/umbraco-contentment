/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
#if NET472
using Umbraco.Core.Models.PublishedContent;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ListItemsEditorDataListItemPropertyValueConverter : IDataListItemPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias == ListItemsDataEditor.DataEditorAlias;
        }

        public IEnumerable<DataListItem> ConvertTo(IPublishedProperty property)
        {
            return property.GetValue() as IEnumerable<DataListItem> ?? Enumerable.Empty<DataListItem>();
        }
    }
}
