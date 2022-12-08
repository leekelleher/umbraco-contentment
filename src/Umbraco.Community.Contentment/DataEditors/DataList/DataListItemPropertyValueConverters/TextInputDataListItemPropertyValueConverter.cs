/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
#if NET472
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class TextInputDataListItemPropertyValueConverter : IDataListItemPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias == TextInputDataEditor.DataEditorAlias;
        }

        public IEnumerable<DataListItem> ConvertTo(IPublishedProperty property)
        {
            var value = property.GetValue() as string;

            return value?
                .ToDelimitedList()
                .Select(x => new DataListItem
                {
                    Name = x.Trim(),
                    Value = x.Trim(),
                })
                ?? Enumerable.Empty<DataListItem>();
        }
    }
}
