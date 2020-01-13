/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Json,
        Group = Core.Constants.PropertyEditors.Groups.RichContent,
        Icon = DataEditorIcon)]
    [Core.Composing.HideFromTypeFinder]
    public sealed class ContentBlocksDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "ContentBlocks";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Content Blocks";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "_empty.html";
        internal const string DataEditorListViewPath = Constants.Internals.EditorsPathRoot + "content-list.html";
        internal const string DataEditorBlocksViewPath = Constants.Internals.EditorsPathRoot + "content-blocks.html";
        internal const string DataEditorOverlayViewPath = Constants.Internals.EditorsPathRoot + "content-blocks.overlay.html";
        internal const string DataEditorIcon = "icon-item-arrangement";

        private readonly IContentService _contentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeService _dataTypeService;
        private readonly Lazy<PropertyEditorCollection> _propertyEditors;

        public ContentBlocksDataEditor(
            ILogger logger,
            IContentService contentService,
            IContentTypeService contentTypeService,
            IDataTypeService dataTypeService,
            Lazy<PropertyEditorCollection> propertyEditors)
            : base(logger)
        {
            _contentService = contentService;
            _contentTypeService = contentTypeService;
            _dataTypeService = dataTypeService;
            _propertyEditors = propertyEditors;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new ContentBlocksConfigurationEditor(
            _contentService,
            _contentTypeService);

        protected override IDataValueEditor CreateValueEditor() => new ContentBlocksDataValueEditor(
            Attribute,
            _contentTypeService,
            _dataTypeService,
            _propertyEditors.Value);
    }
}
