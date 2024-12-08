/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        ValueType = ValueTypes.Bigint,
        ValueEditorIsReusable = true)]
    public sealed class BytesDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "Bytes";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Bytes";
        internal const string DataEditorViewPath = "readonlyvalue";
        internal const string DataEditorIcon = "icon-binarycode";
        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "Bytes";

        private readonly IIOHelper _ioHelper;

        public BytesDataEditor(IIOHelper ioHelper, IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new BytesConfigurationEditor(_ioHelper);
    }
}
