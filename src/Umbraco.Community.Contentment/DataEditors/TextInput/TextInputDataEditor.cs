/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.String,
        Group = Core.Constants.PropertyEditors.Groups.Common,
        Icon = DataEditorIcon)]
    [Core.Composing.HideFromTypeFinder]
    public sealed class TextInputDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "TextInput";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Text Input";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "text-input.html";
        internal const string DataEditorIcon = "icon-autofill";

        private readonly ConfigurationEditorUtility _utility;

        public TextInputDataEditor(ILogger logger, ConfigurationEditorUtility utility)
            : base(logger)
        {
            _utility = utility;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new TextInputConfigurationEditor(_utility);
    }
}
