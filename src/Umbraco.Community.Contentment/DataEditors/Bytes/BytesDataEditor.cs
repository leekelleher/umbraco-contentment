/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Bigint,
        Group = Constants.Conventions.PropertyGroups.Display,
        Icon = DataEditorIcon)]
    public sealed class BytesDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "Bytes";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Bytes";
        internal const string DataEditorViewPath = "readonlyvalue";
        internal const string DataEditorIcon = "icon-binarycode";

        private readonly IIOHelper _ioHelper;

#if NET472
        public BytesDataEditor(IIOHelper ioHelper, ILogger logger)
            : base(logger)
        {
            _ioHelper = ioHelper;
        }
#else
        public BytesDataEditor(IIOHelper ioHelper, IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
        }
#endif

        protected override IConfigurationEditor CreateConfigurationEditor() => new BytesConfigurationEditor(_ioHelper);
    }
}
