/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

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

        public SocialLinksDataEditor(IIOHelper ioHelper, IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new SocialLinksConfigurationEditor(_ioHelper);

        protected override IDataValueEditor CreateValueEditor() => DataValueEditorFactory.Create<JsonArrayDataValueEditor>(Attribute!);
    }
}
