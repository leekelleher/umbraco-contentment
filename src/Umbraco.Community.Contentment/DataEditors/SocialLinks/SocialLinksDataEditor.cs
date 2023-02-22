/* Copyright © 2022 Lee Kelleher.
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
using Umbraco.Cms.Core.Models;
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
        ValueType = ValueTypes.Json,
        Group = UmbConstants.PropertyEditors.Groups.Lists,
        Icon = DataEditorIcon)]
    public class SocialLinksDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "SocialLinks";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Social Links";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "social-links.html";
        internal const string DataEditorOverlayViewPath = Constants.Internals.EditorsPathRoot + "social-links.overlay.html";
        internal const string DataEditorIcon = "icon-molecular-network";

        private readonly IIOHelper _ioHelper;

#if NET472
        public SocialLinksDataEditor(IIOHelper ioHelper, ILogger logger)
            : base(logger)
        {
            _ioHelper = ioHelper;
        }
#else
        public SocialLinksDataEditor(IIOHelper ioHelper, IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
        }
#endif

        protected override IConfigurationEditor CreateConfigurationEditor() => new SocialLinksConfigurationEditor(_ioHelper);

#if NET472
        protected override IDataValueEditor CreateValueEditor() => new JsonArrayDataValueEditor(Attribute);
#else
        protected override IDataValueEditor CreateValueEditor() => DataValueEditorFactory.Create<JsonArrayDataValueEditor>(Attribute);
#endif
    }
}
