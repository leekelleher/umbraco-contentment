/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class JsonArrayDataValueEditor : DataValueEditor
    {
        public JsonArrayDataValueEditor(
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer)
            : base(shortStringHelper, jsonSerializer)
        { }

        public JsonArrayDataValueEditor(
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer,
            IIOHelper ioHelper,
            DataEditorAttribute attribute)
            : base(shortStringHelper, jsonSerializer, ioHelper, attribute)
        { }

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
    }
}
