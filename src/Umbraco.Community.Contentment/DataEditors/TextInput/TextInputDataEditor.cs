/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Strings;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Strings;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.String,
        Group = UmbConstants.PropertyEditors.Groups.Common,
        Icon = DataEditorIcon)]
    public sealed class TextInputDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "TextInput";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Text Input";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "text-input.html";
        internal const string DataEditorIcon = "icon-autofill";

        private readonly IIOHelper _ioHelper;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly ConfigurationEditorUtility _utility;

#if NET472
        public TextInputDataEditor(
            ConfigurationEditorUtility utility,
            IIOHelper ioHelper,
            IShortStringHelper shortStringHelper,
            ILogger logger)
            : base(logger)
        {
            _utility = utility;
            _ioHelper = ioHelper;
            _shortStringHelper = shortStringHelper;
        }
#else
        public TextInputDataEditor(
            ConfigurationEditorUtility utility,
            IIOHelper ioHelper,
            IShortStringHelper shortStringHelper,
            IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
        {
            _utility = utility;
            _ioHelper = ioHelper;
            _shortStringHelper = shortStringHelper;
        }
#endif

        protected override IConfigurationEditor CreateConfigurationEditor() => new TextInputConfigurationEditor(_utility, _ioHelper, _shortStringHelper);
    }
}
