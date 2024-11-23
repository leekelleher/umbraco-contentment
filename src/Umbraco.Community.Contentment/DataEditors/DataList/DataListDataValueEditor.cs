/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class DataListDataValueEditor : DataValueEditor, IDataValueReference
    {
        private readonly IJsonSerializer _jsonSerializer;

        public DataListDataValueEditor(
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer)
            : base(shortStringHelper, jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public override object? ToEditor(IProperty property, string? culture = null, string? segment = null)
        {
            var value = base.ToEditor(property, culture, segment);

            // NOTE: This sets the initial/default value to an empty array, e.g. JSON `[]`.
            // Otherwise it'd be an empty string and cause client-side JavaScript issues, e.g. `"".push()` would error.
            if (value is string str && string.IsNullOrWhiteSpace(str) == true)
            {
                return Array.Empty<object>();
            }

            return value;
        }

        public IEnumerable<UmbracoEntityReference> GetReferences(object? value)
        {
            // NOTE: Due to not being able to access the underlying data-source,
            // at this point we do not know what the value-type references, (it could be anything from a custom data-source).
            // As a halfway compromise, we can take a cursory look at the value (whether it be a single value or array),
            // and if it's a `Udi`, we assume it's an Umbraco reference.
            if (value is string str && string.IsNullOrWhiteSpace(str) == false)
            {
                var items = str.DetectIsJson() == true
                    ? _jsonSerializer.Deserialize<IEnumerable<string>>(str)
                    : str.AsEnumerableOfOne();

                if (items?.Any() == true)
                {
                    foreach (var item in items)
                    {
                        if (item.InvariantStartsWith("umb://") == true &&
                            UdiParser.TryParse(item, out var udi) == true)
                        {
                            yield return new UmbracoEntityReference(udi);
                        }
                    }
                }
            }
        }
    }
}
