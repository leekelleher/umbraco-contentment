/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class GenericDataListItemPropertyValueConverter : IDataListItemPropertyValueConverter
    {
        private readonly string[] _editorAliases = new[]
        {
            DataListDataEditor.DataEditorAlias,
            SocialLinksDataEditor.DataEditorAlias,
            UmbConstants.PropertyEditors.Aliases.MultipleTextstring
        };

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return _editorAliases.InvariantContains(propertyType.EditorAlias) == true;
        }

        public IEnumerable<DataListItem> ConvertTo(IPublishedProperty property)
        {
            if (property.GetValue() is IEnumerable items)
            {
                foreach (var item in items)
                {
                    if (item.TryConvertTo<DataListItem>() is Attempt<DataListItem> attempt &&
                        attempt.Success == true)
                    {
                        yield return attempt.Result;
                    }
                }
            }
        }
    }
}
