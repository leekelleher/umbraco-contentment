/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ReadOnlyDataValueEditor : DataValueEditor
    {
        public override bool IsReadOnly => true;

        public override string ConvertDbToString(PropertyType propertyType, object value, IDataTypeService dataTypeService) => null;

        public override object FromEditor(ContentPropertyData editorValue, object currentValue) => null;

        public override object ToEditor(Property property, IDataTypeService dataTypeService, string culture = null, string segment = null) => null;
    }
}
