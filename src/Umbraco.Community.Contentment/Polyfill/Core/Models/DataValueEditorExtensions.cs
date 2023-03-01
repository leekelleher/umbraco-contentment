/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472 == false
using Umbraco.Cms.Core.Services;

namespace Umbraco.Cms.Core.Models
{
    internal static class DataValueEditorExtensions
    {
        // NOTE: Forwards-compatible polyfill, so that Umb 9+ can support calls that pass-through the `IDataTypeService`.
        // It doesn't do anything with it, just saves me using excessive preprocessors directives.
        public static object ToEditor(
            this IDataValueEditor dataValueEditor,
            Property property,
            IDataTypeService dataTypeService,
            string culture = null,
            string segment = null)
        {
            return dataValueEditor.ToEditor(property, culture, segment);
        }
    }
}
#endif
