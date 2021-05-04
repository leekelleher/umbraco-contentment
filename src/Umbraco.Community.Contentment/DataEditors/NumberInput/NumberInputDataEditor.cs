/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Integer,
        Group = Cms.Core.Constants.PropertyEditors.Groups.Common,
        Icon = DataEditorIcon)]
    internal sealed class NumberInputDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "NumberInput";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Number Input";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "number-input.html";
        internal const string DataEditorIcon = "icon-coin";

        public NumberInputDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new NumberInputConfigurationEditor();
    }
}
