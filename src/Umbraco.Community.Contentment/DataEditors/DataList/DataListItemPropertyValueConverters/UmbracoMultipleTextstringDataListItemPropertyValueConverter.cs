/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
#if NET472
using Umbraco.Core.Models.PublishedContent;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class UmbracoMultipleTextstringDataListItemPropertyValueConverter : IDataListItemPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias == UmbConstants.PropertyEditors.Aliases.MultipleTextstring;
        }

        public IEnumerable<DataListItem> ConvertTo(IPublishedProperty property)
        {
            var value = property.GetValue() as IEnumerable<string>;

            return value?.Any() == true
                ? value.Select(x => new DataListItem
                {
                    Name = x,
                    Value = x,
                })
                : Enumerable.Empty<DataListItem>();
        }
    }
}
