/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models.PublishedContent;
using UmbConstants = Umbraco.Cms.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class TextDelimitedDataListItemPropertyValueConverter : IDataListItemPropertyValueConverter
    {

        private readonly Dictionary<string, char[]> _lookup = new Dictionary<string, char[]>(StringComparer.InvariantCultureIgnoreCase)
        {
            { TextInputDataEditor.DataEditorAlias, UmbConstants.CharArrays.Comma },
            { UmbConstants.PropertyEditors.Aliases.TextArea, UmbConstants.CharArrays.LineFeedCarriageReturn },
            { UmbConstants.PropertyEditors.Aliases.TextBox, UmbConstants.CharArrays.Comma },
        };

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return _lookup.ContainsKey(propertyType.EditorAlias) == true;
        }

        public IEnumerable<DataListItem> ConvertTo(IPublishedProperty property)
        {
            if (_lookup.TryGetValue(property.PropertyType.EditorAlias, out var delimiter) &&
                property.GetValue() is string str &&
                str?.Split(delimiter, StringSplitOptions.RemoveEmptyEntries) is string[] items)
            {
                foreach (var item in items)
                {
                    yield return new DataListItem
                    {
                        Name = item.Trim(),
                        Value = item.Trim(),
                    };
                }
            }
        }
    }
}
