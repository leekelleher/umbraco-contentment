/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

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
    internal sealed class NumberInputDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "NumberInput";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Number Input";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "number-input.html";
        internal const string DataEditorIcon = "icon-coin";

        private readonly IIOHelper _ioHelper;

#if NET472
        public NumberInputDataEditor(IIOHelper ioHelper, ILogger logger)
            : base(logger)
        {
            _ioHelper = ioHelper;
        }
#else
        public NumberInputDataEditor(IIOHelper ioHelper, IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
        }
#endif


        protected override IConfigurationEditor CreateConfigurationEditor() => new NumberInputConfigurationEditor(_ioHelper);
    }
}
