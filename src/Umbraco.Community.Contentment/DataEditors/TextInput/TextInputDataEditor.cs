/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        ValueType = ValueTypes.String,
        ValueEditorIsReusable = true)]
    public sealed class TextInputDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "TextInput";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Text Input";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "text-input.html";
        internal const string DataEditorIcon = "icon-autofill";
        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "TextInput";

        private readonly ConfigurationEditorUtility _utility;

        public TextInputDataEditor(
            ConfigurationEditorUtility utility,
            IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
        {
            _utility = utility;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new TextInputConfigurationEditor(_utility);
    }
}
