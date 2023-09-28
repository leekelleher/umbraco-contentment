/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Collections.Specialized;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class TextboxListDataListItemPropertyValueConverter : IDataListItemPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias == TextboxListDataEditor.DataEditorAlias;
        }

        public IEnumerable<DataListItem> ConvertTo(IPublishedProperty property)
        {
            if (property.GetValue() is NameValueCollection items)
            {
                foreach (var key in items.AllKeys)
                {
                    yield return new DataListItem
                    {
                        Name = key,
                        Value = items[key],
                    };
                }
            }
        }
    }
}
