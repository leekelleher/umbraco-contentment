/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Integer,
        Group = UmbConstants.PropertyEditors.Groups.Common,
        Icon = DataEditorIcon)]
    public sealed class NumberInputDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "NumberInput";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Number Input";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "number-input.html";
        internal const string DataEditorIcon = "icon-coin";

        private readonly IIOHelper _ioHelper;

        public NumberInputDataEditor(IIOHelper ioHelper, IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new NumberInputConfigurationEditor(_ioHelper);
    }
}
