/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
#if DEBUG
        EditorType.PropertyValue, // NOTE: IsWorkInProgress [LK]
#else
        EditorType.Nothing,
#endif
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Json,
        Group = Constants.Conventions.PropertyGroups.RichContent,
#if DEBUG
        Icon = "icon-block color-red"
#else
        Icon = DataEditorIcon
#endif
        )]
    public class ElementDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "Element";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Element";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "element.html";
        internal const string DataEditorOverlayViewPath = Constants.Internals.EditorsPathRoot + "element.overlay.html";
        internal const string DataEditorIcon = "icon-item-arrangement";

        private readonly IContentService _contentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IdkMap _idkMap;

        public ElementDataEditor(ILogger logger, IContentService contentService, IContentTypeService contentTypeService, IdkMap idkMap)
            : base(logger)
        {
            _contentService = contentService;
            _contentTypeService = contentTypeService;
            _idkMap = idkMap;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new ElementConfigurationEditor(_contentService, _contentTypeService, _idkMap);

        protected override IDataValueEditor CreateValueEditor() => new HideLabelDataValueEditor(Attribute);
    }
}
